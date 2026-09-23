# Fleksitid

![.NET](https://img.shields.io/badge/.NET_10-512BD4?logo=dotnet&logoColor=white) ![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white) ![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core_MVC-512BD4?logo=dotnet&logoColor=white) ![SQLite](https://img.shields.io/badge/SQLite-003B57?logo=sqlite&logoColor=white) ![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?logo=bootstrap&logoColor=white) ![Status](https://img.shields.io/badge/status-under_utvikling-orange)

> Videreutvikling av den første, lette [Fleksitid](https://github.com/Thanhibanani/Fleksitid)-appen (ren HTML/CSS/JS). Utviklingslogg dag for dag ligger i [CHANGELOG.md](CHANGELOG.md).

## Mål

Fleksitid skal være et digitalt verktøy for å registrere og følge opp fleksitid i en
virksomhet. Ledere skal ha oversikt over sine konsulenter/medarbeidere, mens den
enkelte selv registrerer egen arbeidstid og sykefravær.

## Fremdrift

- [x] Innlogging og registrering (ASP.NET Core Identity)
- [x] Registrere timer manuelt (dato, antall timer, notat)
- [x] Liste over egne registreringer, med sletting
- [x] Stemple inn/ut med en "Start"/"Slutt"-knapp, som alternativ til manuell registrering
- [ ] 👉 **Her er vi nå:** live klokke/timer på siden mens man er stemplet inn, som
      viser hvor lenge man har jobbet så langt i dag (oppdateres uten å laste siden på nytt)
- [ ] **Arbeidstid**: fast avtalt arbeidstid per periode (f.eks. timer/uke), slik at en
      virksomhet kan ha ulike perioder som vintertid og sommertid. Jobber man mer enn
      avtalt arbeidstid i en periode, gir det fleksitid.
- [ ] Fleksitid-saldo regnet ut fra faktiske timer mot gjeldende arbeidstid-periode
- [ ] Månedlig "registrer"-knapp som låser månedens timer og legger resultatet inn i
      saldoen, slik at gamle registreringer ikke kan endres ved en feil
- [ ] Varsel (ved innlogging) hvis man har glemt å stemple ut en tidligere dag
- [ ] Arkiv med oversikt over tidligere måneders registreringer
- [ ] Leder: oversikt over fleksitiden til sine konsulenter i virksomheten
- [ ] Registrere sykedager for bedre oversikt
- [ ] Se kollegers timer ved samtykke, delt via QR-kode
- [ ] Rollebasert innlogging (leder / medarbeider)

## Teknologi

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQLite (utvikling)
- ASP.NET Core Identity (innlogging og roller)
- Bootstrap 5 (UI, med egen theming via CSS-variabler)

## Kom i gang

```bash
git clone https://github.com/Thanhibanani/Fleksitid-webapp.git
cd Fleksitid-webapp/Fleksitid
dotnet ef database update   # oppretter SQLite-databasen
dotnet run
```

Krever [.NET 10 SDK](https://dotnet.microsoft.com/download) og `dotnet-ef` (`dotnet tool install --global dotnet-ef`).

## Status

Prosjektet er under utvikling. Grunnoppsettet (Identity, database, UI) er på plass,
og tidsregistrering bygges ut fortløpende — se avhuking og pilen under "Fremdrift"
over for hvor langt vi har kommet og hva som er neste.
