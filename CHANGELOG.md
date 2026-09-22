# Endringslogg

Uformell logg over ting som har skjedd underveis — spesielt bugs vi har støtt på og
hvordan de ble løst, så vi husker det senere.

## 2026-09-22 — Tom migrasjon ga en "usynlig" manglende tabell

**Hva skjedde:** `AddArbeidstid`-migrasjonen ble først generert *før*
`DbSet<Arbeidstid>` var lagt til i `ApplicationDbContext` (filen var ikke lagret
ennå), så EF Core så ingen modellendringer og lagde en tom migrasjon
(`Up()`/`Down()` gjorde ingenting). Et forsøk på å generere den på nytt senere
feilet med "navnet er allerede i bruk", og den tomme migrasjonen ble stående —
den ble registrert som "brukt" i databasen uten at `Arbeidstider`-tabellen
faktisk ble opprettet.

**Symptom:** Alt bygde og migrerte tilsynelatende fint (`dotnet ef database update`
sa "already up to date"), men appen ville krasjet i det øyeblikket noen prøvde å
lagre/lese en `Arbeidstid`.

**Fiks:** Fjernet den tomme migrasjonen (`dotnet ef migrations remove`), og
genererte den på nytt etter at modellen faktisk var riktig — denne gangen med
ordentlig `CreateTable`-SQL i `Up()`.

**Lærdom:** Hvis `dotnet ef migrations add` sier "navnet er allerede i bruk", ikke
bare gi den et nytt navn og gå videre — sjekk om den forrige faktisk inneholder
noe (`Up()`-metoden), siden en tom migrasjon kan skjule seg som "up to date".

## Tidligere: samme rotårsak flere ganger

Flere runder med "ulagret fil" (Program.cs, appsettings.json, TimeEntry.cs,
Index.cshtml, Create.cshtml for både Tidsregistrering og Arbeidstid) — endringer
ble skrevet i editoren, men aldri faktisk lagret til disk før commit. Løsning:
skru på Auto Save i editoren (`File > Auto Save` i VS Code).
