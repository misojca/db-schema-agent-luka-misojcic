
# 02 — Evaluacija: Kvantitativne metrike i rezultati

## 1. Metodologija evaluacije

Svaki scenario je evaluiran po pet dimenzija:

| Metrika | Opis |
|---|---|
| **Scenario** | Broj scenarija i tip promene |
| **Složenost** | Osnovna / Srednja / Napredna |
| **Tokeni (credits)** | Credits koje Kiro CLI prijavljuje na kraju agentske sesije. Kiro CLI koristi termin *credits* kao internu meru potrošnje — nije direktno ekvivalentan broju LLM tokena, ali služi kao uporediva metrika između scenarija. |
| **Iteracije do GREEN** | Broj pokretanja DB agenta za isti zadatak |
| **Vreme (min)** | Ukupno vreme od zadatka do prolaska round-trip testa |
| **Ljudske intervencije** | Broj ručnih akcija van agenta |

---

## 2. Kvantitativni rezultati

| # | Scenario | Složenost | Tokeni | Iteracije | Vreme | Intervencije |
|---|---|---|---|---|---|---|
| 1 | Mapiranje postojeće tabele + baseline problem | Osnovna | 5.18 | 2 | 8:05 | 1 |
| 2 | Dodavanje kolone sa default vrednošću | Osnovna | 2.5 | 1 | 3:57 | 0 |
| 3 | Promena tipa kolone (expand-contract) | Srednja | 3.80  | 1 | 6:17  | 0 |
| 4 | Nova šema sa cross-schema FK | Srednja | 3.55  | 1 | 5:30 | 0 |
| 5 | Splitting tabele (expand-contract + data migration) | Napredna | 5.10  | 1 | 6:11| 0 |
| 6 | Merge tabela (istorijski podaci sačuvani) | Napredna | 4.60  | 1 |7:15 | 0 |
| 7 | Dodavanje tabele purchasing.purchasereturn | Osnovna | 2.13| 1 | 4:43 | 0 |
| 8 | Dodavanje kolone tier_expiry_date | Osnovna | 2.22 | 1 | 4:41 | 0 |



### Detaljniji prikaz za Scenario 7 i 8

Pored DB agenta, sledeći agenti su pokrenuti po scenariju:

| Agent | Scenario 7 | Scenario 8 |
|---|---|---|
| Git agent (issue + branch) | 0.22 credits, 37s | 0.22 credits, ~35s |
| DB agent (glavni zadatak) | 2.13 credits, 4m 43s | 2.22 credits, 4m 41s |
| Architecture agent (drift ispravka) | 0.32 credits, 41s | 0.23 credits, 31s |
| **Ukupno po scenariju** | **~2.67 credits** | **~2.67 credits** |

---

## 3. Correctness rate

| Metrika | Vrednost |
|---|---|
| Ukupno scenarija | 8 |
| Prošlo iz prve iteracije | 7 |
| Zahtevalo 2+ iteracije | 1 (Scenario 1 — baseline problem) |
| **Correctness rate iz prve iteracije** | **87.5%** |
| Ukupno generisanih migracija | 10 |
| Round-trip testovi prošli | 10 / 10 |

**Napomena:** Prvih 6 scenarija dalo je correctness rate od 83% (5/6). Dodavanjem Scenarija 7 i 8, koji su oba prošla iz prve iteracije, ukupni correctness rate raste na 87.5% (7/8).

---

## 4. Poređenje sa ručnom implementacijom (baseline)

> **Napomena:** Vrednosti u ovoj sekciji su procene autora zasnovane na iskustvu sa sličnim zadacima, a ne rezultati kontrolisanog merenja. Nisu sprovedeni formalni eksperimenti u kojima bi isti zadatak bio implementiran i agentski i ručno uz merenje vremena.
Kao referentna tačka, analizirana je ručna implementacija zadatka ekvivalentnog Scenariju 7 (dodavanje tabele `purchasing.purchasereturn`):

| Korak | Procena ručnog vremena |
|---|---|
| Čitanje postojeće šeme iz psql | ~3 min |
| Pisanje entity klase (PurchaseReturn.cs) | ~5 min |
| Pisanje Fluent API konfiguracije | ~7 min |
| Pokretanje `dotnet ef migrations add` | ~2 min |
| Provera migracionog fajla | ~3 min |
| Round-trip test | ~3 min |
| Ažuriranje arc42 dokumentacije | ~10 min |
| **Ukupna procena ručne implementacije** | **~33 min** |

DB agent je isti zadatak završio za **4 minute i 43 sekunde** — otprilike **7x brže** od procenjenog ručnog vremena.

Za napredne scenarije (5 i 6), koji zahtevaju expand-contract obrazac sa data migration skriptama, ručna implementacija bi zahtevala dodatno vreme za:
- Dizajn dvofazne migracione strategije (~10 min)
- Pisanje ispravnog CAST i DISTINCT ON SQL-a (~10 min)
- Proveru integriteta podataka između faza (~5 min)

Procenjeno ručno vreme za napredne scenarije: **45–60 minuta**.

---

## 5. Rezultati schema drift senzora

Sensor `agent/sensors/drift-detector.py` pokrenut je nakon svake DB agent akcije tokom demonstracije zatvorene petlje:

| Akcija | Pre Architecture agenta | Nakon Architecture agenta |
|---|---|---|
| Dodavanje kolone `discount_percentage` | 1 COLUMN DRIFT | 0 driftova ✅ |
| Dodavanje tabele `purchaserequest` | 1 TABLE DRIFT | 0 driftova ✅ |
| Brisanje kolone `notes` | 1 COLUMN MISSING | 0 driftova ✅ |
| Dodavanje tabele `purchasereturn` (S7) | 1 TABLE DRIFT | 0 driftova ✅ |
| Dodavanje kolone `tier_expiry_date` (S8) | 1 COLUMN DRIFT | 0 driftova ✅ |

U svakom slučaju, Architecture agent je rešio drift za manje od 45 sekundi.

---

## 6. Ključni nalazi

### 6.1 Autonomno otkrivanje problema

Uočena su dva slučaja autonomnog otkrivanja i rešavanja problema bez ljudske intervencije:

**Baseline problem (Scenario 1):** Agent je pokušao da kreira tabelu `sales.customer` koja već postoji. Round-trip sensor je uhvatio grešku pre commit-a. Prompt agenta je poboljšan obaveznom proverom `__ef_migrations_history`. Druga iteracija je uspešno prošla bez intervencije.

**EF model/baza gap (TDD faza, employees endpoint):** TDD agent je otkrio da `ExcludeFromMigrations()` sprečava primenu pay kolona na živu bazu. Agent je autonomno primenio `ALTER TABLE` i backfill-ovao 290 redova bez ljudske intervencije.

### 6.2 Kontekstualno rasuđivanje

U demonstraciji zatvorene petlje (Akcija 3 — brisanje kolone), agent je bio zamoljeno da obriše kolonu `notes`. Umesto mehaničkog odbijanja jer je DROP COLUMN klasifikovan kao Destructive, agent je procenio kontekst:
- Kolona je bila nullable
- Tabela je imala 0 redova
- Nema rizika od gubitka podataka

Agent je izvršio direktan DROP uz eksplicitno obrazloženje. Ovo demonstrira kontekstualno rasuđivanje, a ne mehaničko praćenje pravila.

### 6.3 Interakcija sa ExcludeFromMigrations()

Uočen je obrazac koji se ponavlja: tabele označene sa `ExcludeFromMigrations()` stvaraju komplikacije kada treba dodati nove kolone. Agent je ispravno identifikovao i rešio ovo u Scenariju 8 uklanjanjem oznake sa `LoyaltyAccountConfiguration`, pošto je `loyaltyaccount` legitimno EF-owned tabela kreirana migracijom.

---

## 7. Ograničenja

### 7.1 Šta agent ne može

| Ograničenje | Opis |
|---|---|
| Performance tuning | Agent primenjuje 3NF po defaultu; ne može da odluči kada je denormalizacija opravdana bez eksplicitne instrukcije |
| Sharding i particioniranje | Nema podrške za PostgreSQL table partitioning ili sharding strategije |
| Kompleksne data transformacije | Agent može da kopira podatke sa CAST ali ne može da implementira business-logic transformacije |
| Kvalitet zavisno od arc42 dokumenta | Kvalitet agenta opada kada je ulazni arc42 dokument neprecizan ili nepotpun |
| Automatska FK index pokrivenost | Agent dodaje indekse na FK kolonama ali ne može da proceni da li su složeni indeksi potrebni |

### 7.2 Threat model

**Rizik: Agent generiše destruktivnu migraciju u produkciji**

Harness ublažava ovaj rizik kroz dva mehanizma:
1. **Klasifikacioni sloj** — svaka promena se klasifikuje kao Safe / Risky / Destructive pre generisanja koda. Destructive promene se odbijaju uz obrazloženje i predlog bezbedne alternative.
2. **Round-trip test** — svaka migracija mora proći apply → rollback → re-apply pre commit-a. Migracija koja ne može da se poništi biva uhvaćena pre nego što dospe u repozitorijum.

**Rizik: Agent zaobilazi safety mehanizme kada tooling daje neočekivan rezultat**

Ovaj rizik se materijalizovao u prvom pokušaju Scenarija 8 (originalni zadatak za `sales.store`), gde je agent ručno pisao u `__ef_migrations_history` kada je `dotnet ef database update` dao ambiguozan rezultat. Preporučena mitigacija: eksplicitno harness pravilo — *ako `dotnet ef database update` da neočekivan rezultat, agent MORA da stane i eskalira ka čoveku umesto da piše direktno u sistemske tabele.*

**Rizik: Narušavanje granica odgovornosti između agenata**

Tokom demonstracije multi-agent feedback loop-a (Issue #19), DB agent je implementirao `LoyaltyController` i Application sloj — što je posao TDD agenta. Preporučena mitigacija: strožija definicija opsega u promptu DB agenta — *DB agent piše isključivo u Domain/Entities/ i Infrastructure/Persistence/; ne sme da kreira niti menja Controllers ili Application layer fajlove.*# 02 — Evaluacija: Kvantitativne metrike i rezultati


