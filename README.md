# Fleksitid

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

## Status

Prosjektet er under utvikling. Grunnoppsettet (Identity, database, UI) er på plass,
og tidsregistrering bygges ut fortløpende — se avhuking og pilen under "Fremdrift"
over for hvor langt vi har kommet og hva som er neste.
