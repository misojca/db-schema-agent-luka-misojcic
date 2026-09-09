# 03 — Demonstracija end-to-end multi-agent feedback loop-a

## Pregled

Ovaj dokument opisuje demonstraciju zatvorenog multi-agent ciklusa sprovedenu u okviru Issue #19. Cilj je bio da se pokaže da svih 6 agenata mogu raditi u koordinisanom lancu gde output jednog agenta postaje input sledećeg — od poslovnog zahteva do zelenog CI/CD pipeline-a.

Kompletni sirovi transcript (shell outputi, diff-ovi generisanog koda, terminal ispisi) dostupan je u `docs/thesis/03-multi-agent-demo-transcript.md`.

---

## Tok demonstracije

### Korak 1 — Requirements agent: generisanje story bundle-a

**Ulaz:** arc42 arhitektonska dokumentacija (`docs/adventureworks-architecture/`)

**Zadatak:** Generisati kompletan user story bundle za Loyalty bounded context.

**Output:** `docs/user-stories/loyalty.md` (401 linija)

Requirements agent je pročitao arc42 Ch.1 (bounded contexts) i Ch.5 (building block view) i generisao priču **LOY-STORY-001: Manage Customer Loyalty Account** sa Pareto klasifikacijom CORE. Bundle sadrži:

- Originalnu priču sa 4 GIVEN-WHEN-THEN scenarija (preuzimanje stanja, kreiranje account-a, kreditovanje poena, odbijanje duplikata)
- **LOY-BE-001.1** — Loyalty Account CRUD API (6 scenarija, GET/POST/PATCH)
- **LOY-BE-001.2** — EF Core entity + Fluent API konfiguracija (2 scenarija)
- **LOY-INFRA-001.1** — ASP.NET Core Web API endpoint (2 scenarija)
- **LOY-INFRA-001.2** — EF Core migracija sa round-trip testom (2 scenarija)
- **LOY-INFRA-001.3** — Referential integrity, cross-schema FK (2 scenarija)
- **LOY-INFRA-001.4** — Monitoring i observability (3 scenarija)

Traceability lanac: `arc42 Ch.5 §5.6 → LOY-STORY-001 → LOY-BE-001.1 → scenariji → testovi`

---

### Korak 2 — Git agent: kreiranje issue-a i grane

**Zadatak:** Kreirati GitHub Issue #19 i feature granu `feat/19-requirements-agent-demo-multi-agent-loop`.

Git agent je generisao issue sa kompletnim acceptance kriterijumima i kreirao granu od master-a. Commit-ovao je story bundle fajlove uz referencu na issue.

---

### Korak 3 — TDD/BDD agent: pisanje RED testa

**Ulaz:** `docs/user-stories/loyalty.md`, fokus na LOY-INFRA-001.2

**Zadatak:** Napisati jedan failing integration test koji proverava `GET /api/loyalty/{customerId}`.

TDD agent je pročitao user story i identifikovao da endpoint ne postoji. Napisao je test `Given_LoyaltyAccountExists_When_GetByCustomerId_Then_ReturnsPointsBalance` koji:

- Poziva `GET /api/loyalty/42`
- Proverava HTTP 200 OK
- Deserializuje response u DTO i proverava da polje `pointsBalance` postoji i da je `>= 0`
- Proverava da se `customerId` ispravno vraća

**Rezultat:** Test pada sa `404 NotFound` — RED potvrđen.

#### Napomena — granica odgovornosti agenta

TDD agent je eksplicitno naveo da ne može direktno da komunicira sa DB agentom:

> *"I'm a coding assistant — I can't send messages to other agents or trigger inter-agent communication. You'd need to route that message to the DB agent yourself."*

Ovo je važan nalaz: **"zatvorena petlja" nije autonomna** — developer ručno prenosi izveštaj o grešci od TDD agenta do DB agenta. Reč je o **human-in-the-loop orkestraciji**, a ne o automatskom handoff-u između agenata. Ovaj nalaz je dokumentovan u sekciji ograničenja u `docs/thesis/02-evaluation.md §7.2`.

TDD agent je ipak generisao precizni feedback report koji developer treba da prosledi DB agentu:

```
FEEDBACK LOOP REPORT — Issue #19
Story: LOY-INFRA-001.2
Test: Given_LoyaltyAccountExists_When_GetByCustomerId_Then_ReturnsPointsBalance
Test status: RED — 404 NotFound
Schema requirement: loyalty.loyaltyaccount mora imati kolonu points_balance
DB Agent action: verifikovati kolonu u bazi, generisati rename migraciju ako je potrebno
```

---

### Korak 4 — DB agent: verifikacija šeme i implementacija

**Ulaz:** Feedback report od TDD agenta (prenet ručno od strane developera)

**Zadatak:** Verifikovati stanje baze i rešiti uzrok pada testa.

DB agent je pokrenuo SQL upit nad `information_schema` i utvrdio:

1. Kolona `points_balance` **već postoji** u `loyalty.loyaltyaccount` — rename migracija nije potrebna
2. `LoyaltyController` ne postoji u API projektu — to je pravi uzrok 404
3. `GetLoyaltyAccountByCustomerIdQuery` i handler ne postoje u Application sloju
4. `IApplicationDbContext` ne eksponira `LoyaltyAccounts`

#### Generisani fajlovi

| Fajl | Svrha |
|---|---|
| `IApplicationDbContext.cs` | Dodat `DbSet<LoyaltyAccount> LoyaltyAccounts` |
| `LoyaltyAccountDto.cs` | Read model sa `PointsBalance` poljem |
| `GetLoyaltyAccountByCustomerIdQuery.cs` | MediatR query record |
| `GetLoyaltyAccountByCustomerIdQueryHandler.cs` | Handler koji čita iz `loyalty.loyaltyaccount` |
| `LoyaltyController.cs` | `GET /api/loyalty/{customerId}` → 200/404 |

DB agent je takođe seed-ovao test podatak direktno u bazu (`customerid=42, points_balance=150`) da bi zadovoljio GIVEN preduslove testa.

#### Napomena — narušavanje granice odgovornosti

DB agent je implementirao `LoyaltyController` i Application sloj — što je posao TDD agenta. Ovo je ograničenje harness arhitekture, ne karakteristika. Preporučena mitigacija je eksplicitno pravilo u DB agent promptu: *DB agent piše isključivo u `Domain/Entities/` i `Infrastructure/Persistence/`.*

---

### Korak 5 — Verifikacija: svi testovi GREEN

Nakon implementacije, pokrenuti su svi integration testovi:

```
Passed! — Failed: 0, Passed: 9, Skipped: 0, Total: 9, Duration: 825ms
```

Svih 9 testova prolazi bez regresija.

---

### Korak 6 — Git agent: commit i PR

Git agent je commit-ovao sve generisane fajlove uz referencu na Issue #19 i push-ovao na feature granu. Kreiran je PR koji je merge-ovan u master.

---

## Vizuelni pregled toka

```
Requirements agent
  └─► LOY-STORY-001 (loyalty.md, 401 linija)
           │
           ▼
      Git agent
  └─► Issue #19 + grana feat/19-...
           │
           ▼
    TDD/BDD agent
  └─► RED test (404 NotFound)
  └─► Feedback report za DB agenta
           │
           ▼ (developer ručno prenosi report)
        DB agent
  └─► Verifikacija šeme (points_balance ✅)
  └─► LoyaltyController + Query + Handler
  └─► Seed podatak (customerid=42)
           │
           ▼
    Svi testovi: 9/9 GREEN ✅
           │
           ▼
      Git agent
  └─► Commit + PR + merge u master
```

---

## Ključni nalazi

| Nalaz | Opis |
|---|---|
| Human-in-the-loop orkestracija | Agenti ne komuniciraju direktno — developer prenosi output jednog agenta kao input sledećem |
| Narušavanje granica odgovornosti | DB agent implementirao Application sloj — posao TDD agenta |
| Autonomna dijagnoza | DB agent sam utvrdio da schema nije problem i identifikovao pravi uzrok (nedostajući Controller) |
| Traceability | Svaki artefakt je trasabilan: arc42 → story → test → kod → commit → PR |
