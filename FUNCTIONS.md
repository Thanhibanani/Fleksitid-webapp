# Funksjonslogg

Dette er en løpende logg over funksjoner vi har bygget i Fleksitid, med en kort
forklaring på *hva* den gjør og *hvordan* den er løst teknisk. Tanken er å
bruke den som læringsnotat underveis, og som dokumentasjon på hva som faktisk
er implementert (i tillegg til fremdriftssjekklisten i [README.md](README.md)).

Nyeste øverst innenfor hver seksjon.

## Innlogging og brukerhåndtering

### Registrering og innlogging (ASP.NET Core Identity)
- **Hva**: Brukere kan registrere seg og logge inn med e-post/passord.
- **Hvordan**: `AddDefaultIdentity<IdentityUser>()` i `Program.cs` setter opp
  Identity med `ApplicationDbContext` som lagring (`AddEntityFrameworkStores`).
  `RequireConfirmedAccount = true` krever bekreftet konto før innlogging.
  Identity-sidene (Login/Register/Manage) kommer ferdig fra
  `Microsoft.AspNetCore.Identity.UI` via `MapRazorPages()`.
- **Filer**: `Program.cs`, `Data/ApplicationDbContext.cs`, `Areas/Identity/`

### Konto-meny i navbar
- **Hva**: Ikon øverst til høyre viser innlogget bruker, med lenke til "Min
  konto" og en logg ut-knapp. Viser i stedet Logg inn/Registrer når man ikke
  er innlogget.
- **Hvordan**: `_LoginPartial.cshtml` injiserer `SignInManager`/`UserManager`
  og sjekker `SignInManager.IsSignedIn(User)` for å bytte innhold i en
  Bootstrap dropdown.
- **Filer**: `Views/Shared/_LoginPartial.cshtml`

## Tidsregistrering

Alt under er i `TidsregistreringController` (krever innlogging via
`[Authorize]`), og bruker `TimeEntry`-modellen (`Id`, `UserId`, `Date`,
`Hours`, `Note`, `StartTime`, `EndTime`).

### Stemple inn/ut (Start/Slutt)
- **Hva**: En "Start"-knapp registrerer at man begynner å jobbe nå. En
  "Slutt"-knapp avslutter og regner ut antall timer automatisk.
- **Hvordan**: `ClockIn()` oppretter en ny `TimeEntry` med `StartTime = Now`
  og `EndTime = null`, men bare hvis brukeren ikke allerede har en åpen
  registrering. `ClockOut()` finner den åpne registreringen (`EndTime ==
  null`), setter `EndTime = Now` og regner `Hours` som differansen i timer
  (`(EndTime - StartTime).TotalHours`). Index-viewet viser "Start" eller
  "Slutt" avhengig av om det finnes en åpen registrering.
- **Filer**: `Controllers/TidsregistreringController.cs` (`ClockIn`,
  `ClockOut`), `Views/Tidsregistrering/Index.cshtml`,
  migrasjon `AddClockTimes`

### Manuell timeregistrering
- **Hva**: Skjema for å legge til en registrering manuelt: dato, antall
  timer (med kvarters presisjon) og valgfritt notat.
- **Hvordan**: `Create()` (GET) forhåndsutfyller dagens dato, `Create(TimeEntry
  entry)` (POST) setter `UserId` til innlogget bruker og lagrer via EF Core.
  `Hours`-feltet bruker `step="0.25"` i inputen for å oppmuntre til
  kvarters-presisjon.
- **Filer**: `Controllers/TidsregistreringController.cs` (`Create`),
  `Views/Tidsregistrering/Create.cshtml`, `Models/TimeEntry.cs`

### Liste over egne registreringer
- **Hva**: "Min tid"-siden viser alle registreringene til den innloggede
  brukeren, nyeste dato først, med timer formatert med to desimaler.
- **Hvordan**: `Index()` henter `TimeEntries` filtrert på `UserId` og sortert
  med `OrderByDescending(e => e.Date)`. Viewet formaterer med
  `Hours.ToString("0.00")`.
- **Filer**: `Controllers/TidsregistreringController.cs` (`Index`),
  `Views/Tidsregistrering/Index.cshtml`

### Slette en registrering
- **Hva**: Hver rad i listen har en "Slett"-knapp.
- **Hvordan**: `Delete(int id)` sjekker at registreringen både finnes *og*
  tilhører innlogget bruker (`e.Id == id && e.UserId == userId`) før den
  fjernes — hindrer at man kan slette andres registreringer ved å gjette en
  id.
- **Filer**: `Controllers/TidsregistreringController.cs` (`Delete`)

## Database

### Datamodell og migrasjoner
- **Hva**: Data lagres i SQLite via Entity Framework Core, med
  versjonskontrollerte migrasjoner.
- **Hvordan**: `ApplicationDbContext` arver `IdentityDbContext` (gir
  bruker/rolle-tabeller gratis) og legger til `DbSet<TimeEntry>`. Tre
  migrasjoner så langt: `InitialCreate` (Identity-tabeller),
  `AddTimeEntry` (tabellen for timeregistreringer), `AddClockTimes`
  (la til `StartTime`/`EndTime`-kolonnene for stemple inn/ut).
- **Filer**: `Data/ApplicationDbContext.cs`, `Data/Migrations/`

## UI / design

### Navbar med sentrert logo
- **Hva**: Navbar med lenker til venstre, "Fleksitid"-logo midt på (uansett
  bredde på lenker/kontomeny), og kontoikon til høyre.
- **Hvordan**: Logoen er tatt ut av flex-flyten med
  `position-absolute top-50 start-50 translate-middle`, mens Bootstrap sin
  `navbar`-container (`display:flex; justify-content:space-between`) plasserer
  lenkene og kontoikonet naturlig i hver sin ende.
- **Filer**: `Views/Shared/_Layout.cshtml`

### Egen theming over Bootstrap
- **Hva**: Bootstrap 5 som base, med egne farger/fonter (DM Sans/DM Mono fra
  Google Fonts) via CSS-variabler i stedet for helt egendefinert CSS.
- **Hvordan**: `fleksi.css` overstyrer Bootstrap sine CSS-variabler og lastes
  etter Bootstrap i `_Layout.cshtml`.
- **Filer**: `wwwroot/css/fleksi.css`, `Views/Shared/_Layout.cshtml`

---

## Forhistorie: statisk prototype

Før .NET-versjonen fantes et rent HTML/CSS/JS-forprosjekt i
[`Fleksitid`](https://github.com/Thanhibanani/Fleksitid)-repoet: én
`index.html`-fil som regnet ut fleksitid (pluss/minus mot en normaldag på
7t45min) og lagret alt i nettleserens `localStorage`, helt uten server. Den
er ikke lenger under aktiv utvikling, men var utgangspunktet for ideen.

## Neste opp

Se ["Fremdrift" i README.md](README.md#fremdrift) for hva som ikke er bygget
ennå (live klokke, fleksitid-saldo, arbeidstidsperioder, leder-visning osv.)
