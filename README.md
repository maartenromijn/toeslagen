# Kinderopvangtoeslag Rekentool

Een complete webapplicatie voor het doorrekenen van kinderopvangkosten, kinderopvangtoeslag en netto besteedbaar inkomen voor de jaren 2026-2030.

## Inhoudsopgave

- [Overzicht](#overzicht)
- [Architectuur](#architectuur)
- [Vereisten](#vereisten)
- [Installatie](#installatie)
- [Gebruik](#gebruik)
- [Functies](#functies)
- [Testen](#testen)
- [Bijdragen](#bijdragen)
- [Licentie](#licentie)

## Overzicht

Deze applicatie vervangt de Excel-rekentool voor kinderopvangtoeslag en biedt:

- Berekening van gezamenlijk toetsingsinkomen
- Inkomen per partner met verlofberekeningen
- Berekening van betaald en onbetaald ouderschapsverlof
- Hypotheekrente en eigenwoningforfait
- Kinderopvangkosten en toeslagberekening
- Eigen bijdrage voor kinderopvang
- Indicatief netto huishoudinkomen
- Vergelijking van scenario's over 2026-2030

## Architectuur

De applicatie volgt een schone architectuur met scheiding van verantwoordelijkheden:

```
ChildcareCalculator/
├── src/
│   ├── ChildcareCalculator.Domain/       # Domeinmodellen en interfaces
│   ├── ChildcareCalculator.Application/   # Applicatielogica en services
│   ├── ChildcareCalculator.Infrastructure/# Data access en externe integraties
│   ├── ChildcareCalculator.Web/          # Blazor webinterface
│   └── ChildcareCalculator.Tests/        # Unit en integratietests
└── ChildcareCalculator.sln               # Solution file
```

### Domeinlaag (Domain)

Bevat alle domeinmodellen, enums en basisclasses:

- **Modellen**: `HouseholdScenario`, `Person`, `AnnualPersonIncome`, `LeavePlan`, `LeavePhase`, `Mortgage`, `MortgagePart`, `ChildcareSettings`, `HomeSettings`, etc.
- **Enums**: `InputCategory`, `LeaveType`, `IncomeType`, `CalculationStatus`, `PolicyValueStatus`, `ChildcareType`
- **Basisclasses**: `EntityBase`, `ValueObject`, `ValueWithCategory`

### Applicatielaag (Application)

Bevat de rekenengine en bedrijfslogica:

- **Services**: 
  - `IncomeCalculationService` - Inkomen berekeningen
  - `LeaveCalculationService` - Verlofberekeningen
  - `MortgageCalculationService` - Hypotheekberekeningen
  - `HomeCalculationService` - Eigen woning berekeningen
  - `AssessmentIncomeService` - Toetsingsinkomen berekeningen
  - `ChildcareCostService` - Kinderopvangkosten berekeningen
  - `ChildcareBenefitService` - Kinderopvangtoeslag berekeningen
  - `RoundingService` - Afrondingslogica
  - `JsonImportExportService` - JSON import/export

- **DTO's**: Data transfer objects voor service communicatie

### Infrastructuurlagen (Infrastructure)

Bevat data access en externe integraties:

- **Entity Framework Core**: Database context en migraties
- **SQLite**: Lokale database voor ontwikkeling
- **PostgreSQL**: Ondersteuning voor productie (configureerbaar)

### Weblaag (Web)

Blazor Server applicatie met:

- **Pagina's**: Dashboard, Scenario's, Huishouden, Inkomen, Verlof, Hypotheek, Kinderopvang, Beleid, Resultaten, Import/Export
- **Componenten**: Herbruikbare UI-componenten
- **Styling**: Bootstrap CSS met aangepaste stijlen

### Testlaag (Tests)

Comprehensive test suite:

- **Unit Tests**: Voor alle rekenengine services
- **Integration Tests**: Voor JSON import/export en database operaties

## Vereisten

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) of hoger
- Een code editor (Visual Studio, VS Code, Rider, etc.)

## Installatie

1. **Clone de repository**:
   ```bash
   git clone https://github.com/maartenromijn/toeslagen.git
   cd toeslagen
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Build de solution**:
   ```bash
   dotnet build
   ```

4. **Voer database migraties uit**:
   ```bash
   dotnet ef database update --project src/ChildcareCalculator.Infrastructure
   ```

5. **Start de applicatie**:
   ```bash
   dotnet run --project src/ChildcareCalculator.Web
   ```

De applicatie is nu beschikbaar op `https://localhost:5001` (of `http://localhost:5000`).

## Gebruik

### Dashboard

De startpagina toont:
- Snelle start opties
- Recente scenario's
- Belangrijke informatie en waarschuwingen

### Scenario's

- **Nieuw Scenario**: Maak een nieuw huishoudscenario aan
- **Bewerken**: Pas een bestaand scenario aan
- **Dupliceren**: Maak een kopie van een scenario
- **Resultaten**: Bekijk berekeningsresultaten
- **Verwijderen**: Verwijder een scenario

### Huishouden

Voer gegevens in voor:
- Naam en beschrijving van het scenario
- Personen in het huishouden
- Basisinstellingen

### Inkomen

Voer inkomensgegevens in per persoon:
- Maandelijks bruto salaris
- Contracturen per week
- Vakantiegeld percentage
- Auto van de zaak (bijtelling)
- Winstuitkering
- Overwinst
- Pensioenpremie
- WIA/WGA premie

### Verlof

Voer verlofplannen in:
- Start- en einddatum
- Type verlof (betaald/onbetaald ouderschapsverlof)
- Werkuren per week
- Verlofuren per week
- Extra uren

### Hypotheek

Voer hypotheekgegevens in:
- Referentiedatum
- Leningdelen met:
  - Oorspronkelijke hoofdsom
  - Restschuld op referentiedatum
  - Rentepercentage
  - Maandelijkse termijn
  - Extra aflossing
  - Renteherzieningsdatum
  - Einddatum

### Kinderopvang

Voer kinderopvanggegevens in:
- Opvangweken per jaar
- Dagen per week
- Uren per dag
- Werkelijk uurtarief
- Maximum uurtarief (voor toeslagberekening)

### Beleid

Bekijk en pas beleidsinstellingen aan:
- Inkomensgrenzen indexfactor
- Middeninkomensopslag
- Toeslag override percentages
- Toeslagtabel

### Resultaten

Bekijk berekeningsresultaten:
- Toetsingsinkomen per jaar
- Kinderopvangkosten en toeslag
- Eigen bijdrage
- Netto inkomen
- Vergelijking tussen jaren
- Grafische weergave

### Import/Export

- **Import**: Laad een scenario uit een JSON-bestand
- **Export**: Sla een scenario op als JSON-bestand
- **Kopieer naar klembord**: Kopieer JSON naar klembord

## Functies

### Rekenengine

De applicatie biedt een complete rekenengine voor:

1. **Inkomen berekeningen**:
   - Jaarlijkse contracturen
   - Normaal dagloon
   - WAZO-daguitkering
   - WAZO-vervangingsratio
   - Salaris na verlof
   - Winstuitkering na verlof
   - Overwinst na verlof
   - Belastbaar inkomen

2. **Verlof berekeningen**:
   - Verlofuren per jaar
   - Betaalde en onbetaalde verlofuren
   - Werkuren tijdens verlof
   - Validatie van verlofplannen

3. **Hypotheek berekeningen**:
   - Maandelijkse aflossingstabel
   - Jaarlijkse resultaten
   - Aftrekbare rente
   - Gemiddelde schuld

4. **Eigen woning berekeningen**:
   - Eigenwoningforfait
   - Saldo eigen woning

5. **Toetsingsinkomen**:
   - Gezamenlijk toetsingsinkomen
   - Indexering van inkomensgrenzen

6. **Kinderopvangtoeslag**:
   - Toeslagpercentage op basis van inkomen
   - Handmatige override
   - Berekening van toeslagbedrag
   - Eigen bijdrage

### Validatie

- Verplichte velden
- Geldige jaartallen
- Startdatum voor einddatum
- Niet-negatieve uren en tarieven
- Percentage tussen 0 en 1
- Contracturen binnen realistische grenzen
- Hypotheekschuld niet negatief

### Afronding

- Geldbedragen intern met voldoende precisie
- Presentatie op 2 decimalen (cents)
- Percentages als decimalen (0-1)
- Expliciete afrondingsregels
- MidpointRounding.AwayFromZero

### Waarschuwingen

De applicatie toont waarschuwingen voor:
- Verlofuren die contracturen overschrijden
- Betaald verlof buiten toegestane periode
- Maximum aantal betaalde verlofuren overschreden
- Overlappende verloffases
- Negatieve uren of tarieven
- Voorlopige beleidswaarden

## Testen

### Unit Tests

Voer unit tests uit voor de rekenengine:

```bash
cd src/ChildcareCalculator.Tests
dotnet test --filter "FullyQualifiedName~ChildcareCalculator.Tests.Unit"
```

### Integration Tests

Voer integratietests uit voor JSON import/export:

```bash
cd src/ChildcareCalculator.Tests
dotnet test --filter "FullyQualifiedName~ChildcareCalculator.Tests.Integration"
```

### Alle Tests

Voer alle tests uit:

```bash
dotnet test
```

## JSON Import/Export

### JSON Structuur

Het JSON-bestand bevat:

- **metadata**: Broninformatie, taal, valuta, jaren
- **inputs**: 
  - **opvang**: Kinderopvang instellingen
  - **woning**: Eigen woning instellingen
  - **beleid**: Beleidsaannames
  - **personen**: Persoonsgegevens en inkomens
- **mortgage**: Hypotheekgegevens
- **leave_plans**: Verlofplannen per persoon
- **extra_income**: Extra inkomsten
- **static_parameters**: Statische parameters (belastingtabelen)
- **formulas**: Berekeningsformules
- **sources**: Bronvermeldingen

### Voorbeeld JSON

Zie `sample.json` voor een compleet voorbeeld van de JSON-structuur.

### Import

1. Selecteer een JSON-bestand
2. Geef een naam op voor het scenario
3. Klik op Importeren

### Export

1. Selecteer een scenario
2. Klik op Exporteren
3. Optioneel: Kopieer naar klembord

## Bijdragen

Bijdragen zijn welkom! Volg deze stappen:

1. Fork de repository
2. Maak een feature branch (`git checkout -b feature/amazing-feature`)
3. Commit je wijzigingen (`git commit -m 'Add amazing feature'`)
4. Push naar de branch (`git push origin feature/amazing-feature`)
5. Open een Pull Request

### Code Stijl

- Gebruik C# 10+ features
- Volg de bestaande code stijl
- Gebruik `decimal` voor alle geldbedragen
- Gebruik `DateOnly` voor datums zonder tijd
- Gebruik nullable reference types
- Voeg unit tests toe voor nieuwe functionaliteit

### Commit Berichten

Gebruik duidelijke commit berichten:

- `feat: add new feature`
- `fix: fix bug`
- `docs: update documentation`
- `test: add tests`
- `refactor: refactor code`
- `chore: maintenance tasks`

## Licentie

Dit project is gelicenseerd onder de MIT License - zie het [LICENSE](LICENSE) bestand voor details.

## Disclaimer

**Belangrijke mededeling:**

Deze berekening is een indicatieve schatting en geen definitieve beschikking van de Belastingdienst, UWV, werkgever of hypotheekverstrekker.

De applicatie gebruikt de meest recente beschikbare beleidsregels en toeslagtabelen. Voor toekomstige jaren (2028-2030) worden voorlopige beleidsaannames gebruikt. Deze kunnen nog wijzigen.

Gebruik de resultaten van deze applicatie op eigen risico. Raadpleeg altijd een professionele adviseur voor financiële beslissingen.

## Contact

Voor vragen of opmerkingen:

- Maak een issue aan in de repository
- Of neem contact op via de project maintainer

---

Gebouwd met ❤️ en .NET 10
