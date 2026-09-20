# Fleksitid

## Mål

Fleksitid skal være et digitalt verktøy for å registrere og følge opp fleksitid i en
virksomhet. Ledere skal ha oversikt over sine konsulenter/medarbeidere, mens den
enkelte selv registrerer egen arbeidstid og sykefravær.

## Planlagte funksjoner

- Innlogging som enten **leder** eller **medarbeider** (rollebasert)
- Leder: oversikt over fleksitiden til sine konsulenter i virksomheten
- Medarbeider: registrere arbeidstid basert på egen kontrakt
- Stemple inn/ut med en "Start"/"Slutt"-knapp, eller legge til timer manuelt
- Se kollegers timer ved samtykke, delt via QR-kode
- Registrere sykedager for bedre oversikt

## Teknologi

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQLite (utvikling)
- ASP.NET Core Identity (innlogging og roller)

## Status

Prosjektet er under utvikling. Grunnoppsettet (Identity, database) er på plass;
funksjonene over bygges ut fortløpende.
