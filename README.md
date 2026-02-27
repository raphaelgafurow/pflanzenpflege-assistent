
# Pflanzenpflege-Assistent

[![.NET Unit Tests](https://github.com/raphaelgafurow/pflanzenpflege-assistent/actions/workflows/dotnet-tests.yml/badge.svg)](https://github.com/raphaelgafurow/pflanzenpflege-assistent/actions/workflows/dotnet-tests.yml)


Desktop-Anwendung zur Verwaltung von Zimmerpflanzen und deren Pflegeintervallen.

## Projektinformationen

- **Autor:** Raphael Gafurow
- **Projektart:** Schulprojekt (Desktop)
- **Technologie:** C#, .NET, Avalonia UI, Entity Framework Core, SQLite

## Projektziel

Mit dem Pflanzenpflege-Assistenten lassen sich Pflanzenstammdaten, Pflege-Regeln und erledigte Pflege-Einträge zentral verwalten.  
Die Anwendung berechnet automatisch den aktuellen Pflege-Status und die nächste fällige Pflege.

## Funktionsumfang

### Pflanzenverwaltung
- Pflanzen anlegen (Name, Art, Standort, Kaufdatum, Notizen)
- Pflanzen löschen
- Übersicht aller Pflanzen

### Pflege-Regeln
- Pflege-Regeln hinzufügen
- Pflege-Regeln ändern
- Pflege-Regeln löschen
- Intervallbasierte Fälligkeitsberechnung (z. B. alle 7 Tage)

  <img width="1624" height="1061" alt="image" src="https://github.com/user-attachments/assets/cedca8a0-631a-4a14-9731-84dbd0bc2eec" />

### Pflege-Einträge
- Pflege-Einträge hinzufügen
- Pflege-Einträge ändern
- Pflege-Einträge löschen
- Historie pro Pflanze

<img width="1624" height="1061" alt="image" src="https://github.com/user-attachments/assets/e037fbc6-87ec-41d5-8bcc-7fd093bfdbfd" />


### Übersicht und Suche
- Pflege-Status: `Überfällig`, `Heute fällig`, `Demnächst fällig`
- Anzeige der nächsten fälligen Pflege inkl. Typ
- Suche/Filter nach Name, Art oder Standort
  
<img width="1624" height="1061" alt="image" src="https://github.com/user-attachments/assets/a089a605-f64d-42c0-9a47-3f1492636bfc" />

## Verwendete Technologien

- **UI:** Avalonia 11
- **Backend/Logik:** .NET (C#)
- **ORM:** Entity Framework Core
- **Datenbank:** SQLite (lokale Datei)

## Voraussetzungen

- Installiertes .NET SDK (Version passend zum Projekt-Target `net10.0`)
- Windows, macOS oder Linux (Avalonia Desktop)

## Projekt starten

1. Repository klonen
2. In das Projektverzeichnis wechseln
3. Desktop-App starten:

```bash
cd Implementierung/PflanzenpflegeAvalonia
dotnet run
```

Beim ersten Start wird die SQLite-Datenbank automatisch erstellt und mit Beispieldaten befüllt.

## Speicherort der Datenbank

Die Datenbank wird lokal im Benutzerprofil abgelegt:

- **Dateiname:** `pflanzenpflege.db`
- **Pfad (logisch):** `%LOCALAPPDATA%/PflanzenpflegeAssistent/` (Windows)  
  bzw. entsprechender `LocalApplicationData`-Pfad des Betriebssystems

## Projektstruktur

- `Implementierung/PflanzenpflegeAvalonia/`  
  Hauptanwendung (Desktop, Avalonia, EF Core, SQLite)
- `Implementierung/PflanzenpflegeKonsole/`  
  Früher Konsolen-Prototyp
- `Planungsdokumente/`  
  Anforderungen, Diagramme, Entwurfsunterlagen
- `Dokumentation/`  
  Projektdokumentation (inkl. KI-Nutzung)

## Screencast

Das folgende Video demonstriert die Nutzung der Anwendung:

[▶ Screencast anschauen](Dokumentation/pflanzenpflege-assistent-screencast.mp4)

