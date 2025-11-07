# Datenbank-Design Pflanzenpflege-Assistent

## Begründung des Datenbankmodells

Mein Datenmodell besteht aus den drei Tabellen **Pflanze**, **Pflegeregel** und **Pflegeeintrag**.  
Damit kann ich speichern, welche Pflanzen ich habe, welche Regeln (gießen oder düngen) dafür gelten und wann die Pflege tatsächlich durchgeführt wurde.

### 1. Tabellenwahl
- **Pflanze:** Grunddaten wie Name, Art, Standort und Kaufdatum.  
- **Pflegeregel:** Wiederkehrende Aufgaben (z. B. gießen alle 7 Tage oder düngen alle 30 Tage).  
- **Pflegeeintrag:** Speichert, wann und wie die Pflege wirklich gemacht wurde.

### 2. Beziehungen
- Eine Pflanze kann mehrere Regeln haben → **1 : n zu Pflegeregel**.  
- Eine Pflanze kann viele Pflegeeinträge haben → **1 : n zu Pflegeeintrag**.  
- Eine Regel gehört immer nur zu einer Pflanze, daher ist keine n : m-Beziehung nötig.

### 3. Schlüssel und Konsistenz
- Jede Tabelle hat einen **Primärschlüssel** (automatisch erzeugte ID).  
- Über **Fremdschlüssel** sind die Tabellen logisch verbunden.  
- Mit **ON DELETE CASCADE** werden alle zugehörigen Datensätze automatisch gelöscht, wenn eine Pflanze entfernt wird.

### 4. Datentypen
- **INT** für IDs und Fremdschlüssel.  
- **VARCHAR** für kurze Texte (z. B. Name, Standort, Typ).  
- **DATE** für alle Datumsangaben.  
- **TEXT** für längere Notizen.

### 5. Unterstützung und Vorgehen
Die Auswahl der Felder und Pflegeintervalle habe ich mit dem Vater meiner Freundin abgestimmt,  
der eine **Ausbildung bzw. Fortbildung als Gärtner** hat und mir erklärt hat, wie man Pflegezyklen realistisch abbildet.  
Die **JOIN-Abfrage** habe ich mit Hilfe einer **KI** erstellt. 
Die Inhalte der Tabellen habe ich dann passend zu den Praxis-Beispielen aus dem Gespräch ergänzt.
