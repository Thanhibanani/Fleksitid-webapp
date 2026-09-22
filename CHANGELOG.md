# Endringslogg

Logg over hva som har skjedd underveis i prosjektet, dag for dag.

## 2026-09-20 — Oppstart

- Bestemte oss for å bygge Fleksitid fra en ren scaffold (`master`-branchen),
  slik at koden skrives selv, med veiledning underveis — i stedet for at alt
  blir generert.
- La til `README.md` med mål og planlagte funksjoner for prosjektet.
- Byttet database fra SQL Server LocalDB til SQLite: NuGet-pakke byttet ut,
  `Program.cs` (`UseSqlServer` → `UseSqlite`), `appsettings.json`
  (connection string).
- Ny `TimeEntry`-modell (dato, timer, notat) og migrasjon for den.
- Startet på frontend: kopierte `fleksi.css` fra den gamle statiske
  prototypen som utgangspunkt for styling.

## 2026-09-21 — Tidsregistrering, navbar-redesign, stempling

- Oppdaget at `fleksi.css` sin globale reset kolliderte visuelt med
  Bootstrap-navbaren. Løsning: droppet egne CSS-klasser og gikk over til å
  overstyre Bootstrap 5.3 sine `--bs-*`-CSS-variabler (fonter, farger,
  border-radius) i stedet — enklere å vedlikeholde og fortsatt responsivt.
- Bygget `TidsregistreringController` + views: `Index` (liste), `Create`
  (skjema for manuell registrering), `Delete`.
- La til navbar-lenke til "Min tid".
- Redesignet navbar med inspirasjon fra en referanseside brukeren likte:
  sentrert logo (via `position-absolute` + `translate-middle`), nav-lenker
  til venstre, konto-ikon (Bootstrap Icons, `person-circle` i en
  nedtrekksmeny) til høyre i stedet for tekstlenker.
- Formaterte `Hours`-input som et tallfelt (`type="number"`, steg på 0,25t)
  og satt standard dato til i dag i `Create`-skjemaet.
- Bygget stempling: la til `StartTime`/`EndTime` på `TimeEntry`, og
  `ClockIn`/`ClockOut`-actions med "Start"/"Slutt"-knapper på
  Tidsregistrering-siden.
- README oppdatert flere ganger: detaljert funksjonsliste, og til slutt
  gjort om til en ekte fremdriftssjekkliste (`- [x]`) med en 👉-pil som
  peker på hva som er neste.
- Diskuterte og landet på design for veien videre: månedlig låsing av
  registreringer inn i en saldo, et varsel-banner (ikke ekte push-varsel)
  ved innlogging om glemt utstempling, arkiv for tidligere måneder, og en
  `Arbeidstid`-modell for å støtte vintertid/sommertid-perioder.
- Avklarte at leder–konsulent-forholdet ikke trenger en egen
  samtykke-/håndhilse-mekanisme (det er strukturelt del av å bli med i
  virksomheten) — samtykke er forbeholdt frivillig kollega-til-kollega-deling.

## 2026-09-22 — Home-ikon, Arbeidstid, og en skikkelig bug

- Byttet "Home"-teksten i navbaren med et hus-ikon.
- Bygget `Arbeidstid`-modell (navn, timer/uke, fra-/til-dato), controller og
  views for å registrere arbeidstid-perioder (f.eks. "Sommertid" 30t/uke).
- **Bug funnet og fikset:** `AddArbeidstid`-migrasjonen ble første gang
  generert før `DbSet<Arbeidstid>` faktisk var lagret i
  `ApplicationDbContext`, så den ble tom (`Up()`/`Down()` gjorde ingenting).
  Et nytt forsøk på å generere den feilet med "navnet er i bruk", og den
  tomme migrasjonen ble stående — registrert som "anvendt" uten at
  `Arbeidstider`-tabellen faktisk ble opprettet. Alt så bra ut
  (`dotnet ef database update` sa "already up to date"), men appen ville
  krasjet i det øyeblikket noen prøvde å bruke en `Arbeidstid`.
  Funnet ved å faktisk teste hele flyten (registrere → logge inn → lage en
  periode) i stedet for å stole på grønt build. Fikset ved å fjerne den
  tomme migrasjonen og generere den på nytt med riktig `CreateTable`-SQL.
- To views (`Views/Arbeidstid/Index.cshtml` og `Create.cshtml`) kom
  tomme/manglende fra en tidligere push — fylt inn på nytt.

## Gjentakende ting å huske på

- **Lagre filen før du bygger.** Flere runder med ulagrede endringer
  (`Program.cs`, `appsettings.json`, `TimeEntry.cs`, diverse `.cshtml`-filer)
  som så riktige ut i editoren, men var tomme/gamle på disk. Løsning: skru
  på Auto Save (`File > Auto Save` i VS Code).
- **Appen låser exe-fila om den kjører i bakgrunnen** når du prøver å bygge
  eller migrere i et annet vindu. Kjør `taskkill /F /IM Fleksitid.exe` før
  `dotnet build`/`dotnet ef` for å være trygg, uansett om du husker om
  appen faktisk kjører.
- Kjør alltid kommandoer fra `Fleksitid`-undermappa, ikke repo-roten
  (`Fleksitid-webapp`) — det er der `.csproj`-fila ligger.
