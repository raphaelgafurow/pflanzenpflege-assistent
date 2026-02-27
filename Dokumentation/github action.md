# GitHub Action fuer automatische Unittests

## Ziel
Bei jedem Push ins Repository werden die Unittests automatisch durch GitHub Actions ausgefuehrt.

## Umgesetzte Schritte
1. Im Repository den Ordner `.github/workflows/` erstellt.
2. Die Workflow-Datei `.github/workflows/dotnet-tests.yml` angelegt.
3. Als Trigger `push` und `pull_request` konfiguriert.
4. In der Action folgende Schritte definiert:
   - Repository auschecken (`actions/checkout@v4`)
   - .NET SDK installieren (`actions/setup-dotnet@v4`, Version `10.0.x`)
   - NuGet-Abhaengigkeiten wiederherstellen (`dotnet restore`)
   - Unittests ausfuehren (`dotnet test` auf dem Testprojekt)
5. Workflow-Datei committen und in GitHub pushen.
6. Unter dem Tab **Actions** pruefen, ob der Lauf erfolgreich war.
7. README um einen Status-Badge ergaenzt, der den letzten Testlauf anzeigt.

## Verwendete Dateien
- `.github/workflows/dotnet-tests.yml`
- `README.md`

## Testprojekt
Ausgefuehrt wird:
- `Implementierung/PflanzenpflegeAvalonia.Tests/PflanzenpflegeAvalonia.Tests.csproj`

## Badge
Verwendeter Badge-Link:
- `https://github.com/raphaelgafurow/pflanzenpflege-assistent/actions/workflows/dotnet-tests.yml/badge.svg`
