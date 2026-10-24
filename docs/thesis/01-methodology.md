# Razvoj agenta za projektovanje i evoluciju relacionih baza podataka u okviru multi-agent harness arhitekture

*Pregled toka rada — šta je urađeno i zašto*

---

## 1. Uvod i tema master rada

Master rad se bavi razvojem šestog agenta koji dopunjuje postojeći multi-agent harness namenjen AI-asistiranom softverskom razvoju. Kurs "Software Development Processes Powered by AI Agents" definiše pet Kiro CLI agenata — Git, Architecture, Requirements, CI/CD i TDD/BDD — koji zajedno automatizuju najveći deo softverskog razvojnog procesa. Nijedan od ovih pet agenata ne pokriva relacionu bazu podataka: projektovanje šeme, evoluciju šeme kroz vreme i konzistentnost između koda i baze.

Ta praznina je polazna tačka rada. Doprinos je DB agent (M6) — agent specijalizovan za projektovanje i evoluciju relacione šeme unutar .NET/EF Core projekta, koji prati istu arhitekturu (JSON konfiguracija + system prompt + alati) kao i preostalih pet agenata, čime se čuva metodološka konzistentnost celog harness-a.

**Naslov teme:** "Razvoj agenta za projektovanje i evoluciju relacionih baza podataka u okviru multi-agent harness arhitekture za AI-asistirani softverski razvoj"

### 1.1 Izbor tehnologija i motivacija

- **AdventureWorks** kao demo baza — realna, složena PostgreSQL baza sa 68 tabela u 5 šema (kasnije 6, dodavanjem loyalty šeme), umesto izmišljene baze. Agent mora da razume postojeći kontekst pre nego što generiše promene, što je realističniji scenario od projektovanja od nule.
- **.NET 8 / EF Core 8** kao stack — bliže industrijskoj praksi nego Python/AWS stack korišćen u kursu, čime se testira prenosivost harness principa na drugu tehnologiju.
- **Kiro CLI** kao alat za pokretanje agenta — isti alat kao u kursu, radi metodološke konzistentnosti.

---

## 2. Postavljena infrastruktura

Pre razvoja samog DB agenta postavljen je kompletan projekat koji služi kao okruženje u kojem agent radi.

### 2.1 GitHub repozitorijum

Kreiran je nov, zaseban repozitorijum `db-schema-agent-luka-misojcic` za master rad, sa istim Git workflow konvencijama kao u kursu: GitHub Issue → feature grana → commit-ovi → Pull Request → merge u master.

### 2.2 AdventureWorks PostgreSQL baza

AdventureWorks baza je podignuta u Docker kontejneru na portu 5433, sa 68 tabela raspoređenih u 5 (kasnije 6) šema: `humanresources`, `person`, `production`, `purchasing`, `sales`, i naknadno dodata `loyalty` šema.

### 2.3 .NET Clean Architecture projekat

Napravljen je .NET 8 solution sa 7 projekata organizovanih po Clean Architecture / DDD principima:

- `AdventureWorks.Domain` — entiteti, bez zavisnosti od EF Core
- `AdventureWorks.Application` — CQRS use case-ovi (MediatR)
- `AdventureWorks.Infrastructure` — EF Core DbContext, Fluent API konfiguracije, migracije
- `AdventureWorks.Api` — ASP.NET Core kontroleri
- 3 test projekta (uključujući Integration Tests)

EF Core 8 je povezan sa AdventureWorks bazom i inicijalni integration testovi prolaze.

### 2.4 arc42 arhitektonska dokumentacija

Napisan je arc42 dokument za AdventureWorks (4 poglavlja) koji služi kao ulazni kontekst za DB agenta — opisuje 5 bounded context-a, cross-schema zavisnosti i 3 Architecture Decision Record-a (ADR). DB agent automatski učitava ovaj dokument pre svake akcije.

### 2.5 Preneseni agenti M1–M5

Iz starog repozitorijuma (`sdp-powered-by-ai-agents-luka-misojcic`) preneto je tačno stanje na kraju Modula 5 (bez Modula 6 projekta iz kursa) — svih pet agenata i pripadajuća arhitektonska dokumentacija, kako bi DB agent imao realan kontekst u koji se uklapa unutar postojećeg pipeline-a agenata.

---

## 3. DB agent — arhitektura i dizajn

DB agent je, kao i preostalih pet agenata, Kiro CLI agent — sledi isti obrazac: JSON konfiguracija → system prompt → alati (`fs_read`, `fs_write`, `shell`). Jedina razlika u odnosu na ostale agente je sadržaj prompta i shell komande koje poziva.

### 3.1 Dva ključna fajla

**`db-agent.json` — konfiguracija agenta**

- `prompt: file://./db-agent-prompt.md` — system prompt živi u zasebnom fajlu (isti pristup kao TDD agent iz kursa, jer je prompt kompleksan)
- `tools` — agent može da čita fajlove, piše fajlove i pokreće shell komande (`dotnet ef`, `psql`)
- `allowedTools: ["fs_read"]` — čitanje je auto-odobreno; pisanje i shell komande traže odobrenje korisnika
- `resources` — arc42 dokumenti se automatski učitavaju u kontekst agenta pri svakom pokretanju
- `postToolUse` hook — posle svakog fajla koji agent napiše, automatski se pokreće round-trip test migracije

**`db-agent-prompt.md` — system prompt (srce agenta)**

- Workflow — tačan redosled koraka: čitaj šemu → klasifikuj promenu → generiši C# → generiši migraciju → generiši ER dijagram → izveštaj
- Read Schema Command — gotove SQL komande za čitanje `information_schema` iz AdventureWorks baze
- Change Classification — Safe (generiši direktno) / Risky (expand-contract obrazac) / Destructive (odbij i predloži alternativu)
- Folder Structure — tačna pravila gde ide svaki generisani fajl
- Critical Rules — apsolutna pravila (nikad direktan DROP, uvek indeks na FK koloni, uvek `snake_case` za PostgreSQL, max 3 iteracije u feedback loop-u)

### 3.2 Harness model — guides i sensors

Dizajn direktno prati harness metodologiju kursa, primenjenu na novi domen (evolucija šeme baze podataka).

**Guides (feedforward, u system promptu)**

- Konvencije imenovanja: `snake_case` za PostgreSQL kolone, `PascalCase` za C# klase
- Normalizacija: podrazumevano 3NF, izuzeci moraju biti obrazloženi
- Backward compatibility: zabranjen direktan `DROP COLUMN`, obavezan expand-contract
- FK pravila: sve FK kolone moraju imati indeks

**Sensors (feedback, hooks)**

- Round-trip test migracije (apply → rollback → re-apply)
- FK integrity provera
- Schema drift detekcija (poklapanje arc42 dokumenta i stvarnog stanja baze)

### 3.3 Granice agenta

DB agent ne piše celu aplikaciju — radi isključivo na Domain sloju (entity klase) i Infrastructure sloju (DbContext konfiguracije, migracije). Application i API sloj ostaju u domenu TDD/BDD agenta. Agent takođe ne donosi arhitektonske odluke — korisnik definiše šta treba dodati, agent zna kako to ispravno implementirati.

### 3.4 Context engineering — 4 TODO sekcije

Prompt je inicijalno sadržao 4 TODO sekcije koje je trebalo eksplicitno popuniti:

- **Naming Conventions** — tačna pravila imenovanja za C#, PostgreSQL, migracije, namespace-ove
- **Expand-Contract Template** — konkretan C# template sa `Up()` i `Down()` metodama za dvofazne migracije
- **Mermaid ER Diagram Template** — format ER dijagrama koji agent generiše
- **Feedback Loop Rules** — šta agent radi kada TDD test padne zbog problema u šemi

---

## 4. Evaluacioni scenariji — pregled i tok

Definisano je i sprovedeno 8 evaluacionih scenarija različite složenosti. Svaki scenario je sproveden kroz identičan Git workflow: GitHub Issue → feature grana → DB agent generiše kod i migraciju → round-trip test → commit → push/merge.

| # | Tip promene | Složenost | Iteracije | Migracije | Round-trip |
|---|---|---|---|---|---|
| 1 | Mapiranje postojeće tabele | Osnovna | 2 | 1 (prazna baseline) | ✅ |
| 2 | Dodavanje kolone sa default vrednošću | Osnovna | 1 | 1 | ✅ |
| 3 | Promena tipa kolone (real → numeric) | Srednja | 1 | 2 (expand+contract) | ✅ |
| 4 | Nova šema + cross-schema FK | Srednja | 1 | 1 | ✅ |
| 5 | Splitting tabele (normalizacija) | Napredna | 2 | 2 (expand+contract) | ✅ |
| 6 | Merge tabela (istorijski podaci sačuvani) | Napredna | 2 | 2 (expand+contract) | ✅ |

Ukupno: 11 migracija generisano za 8 scenarija. Correctness rate iz prve iteracije iznosi **62.5%** (5/8). Scenario 1 zahtevao je 2 iteracije zbog otkrivenog baseline problema — round-trip sensor je uhvatio grešku pre commit-a, što je dovelo do iterativnog poboljšanja prompta agenta. Scenariji 5 i 6 ponovljeni su jer prvi pokušaji nisu ostavili ispravne migracije (PR #46 i PR #47). Svi round-trip testovi su na kraju prošli uspešno.

### 4.1 Scenario 1 — Mapiranje postojeće tabele purchaseorderdetail

Cilj: mapirati postojeću tabelu `purchasing.purchaseorderdetail` u EF Core projekat.

Prva iteracija je otkrila kritičan problem: agent nije proveravao `__ef_migrations_history` pre generisanja migracije. EF Core je pokušao da kreira tabelu `sales.customer` koja već postoji, i round-trip test je pao sa greškom `42P07: relation "customer" already exists`. Ovo je tzv. **baseline problem** — baza već postoji sa strukturom, ali EF Core migracije tek počinju da prate promene.

Rešenje: prompt agenta je proširen obaveznom proverom `__ef_migrations_history` pre generisanja migracije. Ako je istorija prazna a DbContext ima registrovane entitete koji mapiraju postojeće tabele, agent mora primeniti `ExcludeFromMigrations()` pre nastavka. Druga iteracija je bila potpuno uspešna.

**Akademska vrednost**: round-trip sensor je uhvatio grešku pre commit-a, ograničenje agenta je otkriveno i iterativno rešeno, što je prirodan i vredan deo istraživačkog procesa.

### 4.2 Scenario 2 — Dodavanje kolone weight_kg u production.product

Cilj: dodati nullable kolonu `weight_kg` tipa `numeric(10,2)`, default 0.0.

Klasifikacija: Safe. Agent je generisao entity property, Fluent API konfiguraciju i migraciju sa stvarnim DDL-om. Round-trip test prošao iz prve.

Tokom ovog scenarija otkrivena je i ispravljena greška u proceduri round-trip testiranja: `dotnet ef migrations remove --force` briše migracioni fajl sa diska ali ne briše zapis iz `__ef_migrations_history`. Ispravna procedura koristi `dotnet ef database update 0`.

### 4.3 Scenario 3 — Promena tipa kolone averagerate (real → numeric)

Cilj: promena tipa `sales.currencyrate.averagerate` iz `real` u `numeric(19,4)` radi finansijske preciznosti.

Klasifikacija: Risky — zahteva expand-contract u dve faze: (1) expand faza dodaje novu kolonu i kopira podatke uz CAST; (2) contract faza briše staru kolonu i preimenuje novu. Round-trip test prošao iz prve za obe migracije.

### 4.4 Scenario 4 — Nova loyalty šema sa cross-schema FK

Cilj: dodati novu PostgreSQL šemu `loyalty` sa tabelom `loyaltyaccount`, uz cross-schema foreign key prema `sales.customer`.

Klasifikacija: Safe. Agent je generisao `EnsureSchema()`, `CreateTable()` sa cross-schema FK, indeks na FK koloni i unique constraint. Round-trip test prošao iz prve.

### 4.5 Scenario 5 — Splitting tabele person.address

Cilj: razdvojiti `person.address` na `person.address` (osnovni podaci) i `person.addressdetail` (prostorni podaci), eliminišući mešanje odgovornosti.

Klasifikacija: Advanced/Risky — expand-contract sa data migration. Expand faza kreira novu tabelu i migrira `spatiallocation` podatke, contract faza uklanja kolonu iz originalne tabele. Uspostavljena one-to-one relacija. Prvi pokušaj (PR #13) nije ostavio ispravne migracije u repozitorijumu, pa je scenario ponovljen (PR #46). U ponovljenom pokušaju agent je otkrio pogrešno `ExcludeFromMigrations()` na `addressdetail` i zavisni view `pe.a`, koji je obrisao i ponovo kreirao bez `CASCADE`. Round-trip test je prošao.

### 4.6 Scenario 6 — Merge tabela (employee + employeepayhistory)

Cilj: denormalizacija — dodavanje kolona `currentpayrate`, `currentpayfrequency` i `paychangedate` direktno u `employee` tabelu, uz migraciju najnovijih podataka iz `employeepayhistory`.

Klasifikacija: Advanced. Za razliku od prethodnih scenarija, contract faza **nije obrisala** `employeepayhistory` tabelu — ona ostaje nepromenjena kao istorijski zapis. Expand faza dodaje tri kolone i popunjava ih podupitom nad `MAX(ratechangedate)`; contract faza postavlja `NOT NULL`. Prvi pokušaj nije ostavio ispravne migracije (pay kolone dodate su direktnim `ALTER TABLE`, videti 6.2), pa je polazno stanje resetovano (commit `a798ea5`) i scenario ponovljen (PR #47). Round-trip test je prošao.

---

## 5. Schema drift sensor — zatvorena petlja dokumentacija/kod

Implementiran je Python sensor (`agent/sensors/drift-detector.py`) koji automatski upoređuje arc42 arhitektonsku dokumentaciju sa stvarnim stanjem PostgreSQL baze i detektuje neusklađenosti.

### 5.1 Kako sensor radi

- Čita `docs/adventureworks-architecture/05-building-block-view.md` i parsira dokumentovane šeme, tabele i kolone
- Konektuje se na bazu i upituje `information_schema` za stvarno stanje
- Upoređuje dva skupa i identifikuje razlike: nedokumentovane tabele/kolone u bazi, dokumentovane koje ne postoje u bazi, nedokumentovane šeme
- Generiše drift report i čuva ga kao `.txt` fajl

### 5.2 Testiranje zatvorene petlje kroz 3 akcije

| Akcija | DB agent | Sensor | Architecture agent | Sensor ponovo |
|---|---|---|---|---|
| 1 — Nova kolona | Dodao `discount_percentage` | Detektovao COLUMN DRIFT | Ažurirao arc42 | 0 driftova ✅ |
| 2 — Nova tabela | Dodao `purchaserequest` | Detektovao TABLE DRIFT | Ažurirao arc42 | 0 driftova ✅ |
| 3 — Brisanje kolone | Obrisao `notes` (sa obrazloženjem) | Detektovao COLUMN MISSING | Uklonio iz arc42 | 0 driftova ✅ |

**Posebno akademski značajna treća akcija**: agent nije mehanički odbio brisanje kao Destructive — procenio je kontekst (nullable kolona, tabela bez podataka) i zaključio da expand-contract ne donosi bezbednosnu vrednost. Ovo pokazuje kontekstualno rasuđivanje, ne mehaničko praćenje pravila.

---

## 6. Proširenje TDD/BDD agenta, API sloj i preostali agenti

### 6.1 GET /api/products (Issue #17)

Sproveden RED → GREEN → REFACTOR ciklus: napisan integration test korišćenjem `WebApplicationFactory<Program>`, kreirani `GetAllProductsQuery`, `ProductDto`, `IApplicationDbContext`, `GetAllProductsQueryHandler` i `ProductsController`. Svih 6 testova prolazi.

### 6.2 GET /api/employees

Analogan ciklus za endpoint zaposlenih. Tokom GREEN faze TDD agent je sam dijagnostikovao i rešio problem — `ExcludeFromMigrations()` iz Scenarija 1 sprečio je primenu pay kolona iz Scenarija 6 na živu bazu. Agent je primenio direktan `ALTER TABLE ... ADD COLUMN IF NOT EXISTS` i backfill-ovao 290 zaposlenih. Svih 8 testova prolazi. Ovaj pristup je zaobišao migracije, pa su u ponovljenom scenariju 6 kolone uklonjene (commit `a798ea5`) i dodate kroz expand-contract migracije (PR #47).

**Akademska napomena**: konkretan primer feedback loop-a unutar TDD agenta — otkriven i rešen pre-existing gap između EF modela i baze.

### 6.3 Adaptacija Requirements agenta

Requirements agent je adaptiran sa Parking Lot (Python/FastAPI) na AdventureWorks (.NET/EF Core/PostgreSQL) kontekst. Pokrenut je i generisao kompletan story bundle za Loyalty bounded context (LOY-STORY-001) — originalnu priču sa 4 GIVEN-WHEN-THEN scenarija, dekomponovanu na BE i INFRA sub-priče. Priča je iskorišćena kao osnova za demonstraciju zatvorenog multi-agent feedback loop-a.

### 6.4 Adaptacija CI/CD agenta i pipeline

CI/CD agent je adaptiran na .NET stack i generisao:

- **Dockerfile** — multi-stage build (`sdk:8.0` za build, `aspnet:8.0` za runtime)
- **`.github/workflows/ci.yml`** — seed restore → dotnet restore → dotnet build → dotnet ef database update → round-trip test → dotnet test → schema drift sensor

Agent je sam rešio problem AdventureWorks seed-a: lokalni Docker image nije na registry-ju, pa je generisao `scripts/adventureworks-seed.sql.gz` (18MB, 71 CREATE TABLE, 71 COPY blokova) pokretanjem `pg_dump` unutar kontejnera. CI pipeline je zelen.

### 6.5 Demonstracija zatvorenog multi-agent feedback loop-a (Issue #19)

Sproveden end-to-end multi-agent ciklus:

```
Requirements agent → LOY-STORY-001 (loyalty story bundle)
        ↓
TDD agent → RED test (404 NotFound, GET /api/loyalty/{customerId})
        ↓
Strukturisani izveštaj → DB agent
        ↓
DB agent → verifikacija šeme + implementacija LoyaltyController + GREEN
        ↓
CI/CD pipeline → svih 9 testova prolazi, pipeline zelen
        ↓
Git agent → commit i PR


**Napomena — narušavanje granica odgovornosti:** DB agent je u ovom scenariju implementirao `LoyaltyController` i Application sloj (`GetLoyaltyAccountByCustomerIdQuery`, handler, DTO) — što je posao TDD agenta, ne DB agenta. Ovo se desilo jer DB agent ima pristup celom codebase-u i prepoznao je da je šema ispravna ali da nedostaje aplikacioni sloj. Ovo je ograničenje trenutne implementacije, ne karakteristika — preporučena mitigacija je strožija definicija opsega u promptu DB agenta.

**Napomena — human-in-the-loop orkestracija:** TDD agent je eksplicitno naveo da ne može direktno da komunicira sa DB agentom ("I can't send messages to other agents"). Strukturisani izveštaj je ručno prenet od strane developera. Dakle "zatvorena petlja" nije autonomna — reč je o human-in-the-loop orkestraciji gde developer igra ulogu orkestratora između agenata.
```

**Ključni nalaz**: granica odgovornosti između TDD i DB agenta u praksi nije uvek oštra — dijagnoza problema često zahteva uvid u više slojeva. Ovo je vredna tema za diskusiju u radu.

---

## 7. Sumarni rezultati

- **11 migracija** generisano kroz 8 scenarija, correctness rate **62.5%** iz prve iteracije (5/8 scenarija — scenariji 1, 5 i 6 zahtevali su 2 iteracije)
- Svi round-trip testovi (apply → rollback → re-apply) prošli uspešno
- Zatvorena petlja schema drift senzora demonstrirana kroz 3 nezavisne akcije, 0 driftova nakon svake
- **9 integration testova** prolazi za API sloj (GET /api/products, GET /api/employees, GET /api/loyalty/{customerId})
- 2 slučaja autonomnog otkrivanja i rešavanja problema: baseline problem (Scenario 1) i EF model/baza gap (TDD faza employees)
- 1 slučaj kontekstualnog rasuđivanja: brisanje notes kolone (Destructive → direktan DROP uz obrazloženje)
- 1 demonstriran end-to-end multi-agent feedback loop (Requirements → TDD → DB → CI/CD → Git)

---

## 8. Tehnički stack

| Komponenta | Tehnologija |
|---|---|
| Agent runtime | Kiro CLI |
| Aplikacioni stack | .NET 8 / EF Core 8 / MediatR |
| Baza podataka | PostgreSQL (AdventureWorks u Docker kontejneru) |
| Sensor | Python (drift-detector.py) |
| Testovi | xUnit (integration testovi) |
| CI/CD | GitHub Actions |
