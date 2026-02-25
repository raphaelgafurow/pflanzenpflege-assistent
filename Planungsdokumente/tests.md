# Testfälle

##ID: T01 – Pflanze anlegen (Pflichtfelder + Anzeige)

**Beschreibung:** Eine neue Pflanze wird angelegt und erscheint in der Übersicht. Pflichtfelder werden validiert.  
**Vorbedingungen:** Anwendung ist gestartet, Datenbank wurde initialisiert, Übersichtsansicht ist sichtbar.  
**Test-Schritte:**
1. „Pflanze anlegen“ öffnen.
2. Name leer lassen und Speichern/Bestätigen drücken.
3. Prüfen, ob eine verständliche Fehlermeldung erscheint.
4. Name = „Monstera“, Art = „Monstera deliciosa“, Standort = „Wohnzimmer“, Kaufdatum setzen (gültiges Datum), Notiz optional.
5. Speichern/Bestätigen.
6. In der Übersicht prüfen, ob die Pflanze angezeigt wird.

**Erwartetes Resultat:**
- Schritt 2–3: Speichern wird verhindert, Fehlermeldung erscheint.
- Schritt 4–6: Pflanze wird gespeichert und ist in der Übersicht sichtbar.


## ID: T02 – Pflege bestätigen (Eintrag + nächste Fälligkeit)

**Beschreibung:** Eine Pflege wird bestätigt und das nächste Fälligkeitsdatum wird aktualisiert.  
**Vorbedingungen:** Es existiert eine Pflanze mit mindestens einer Pflege-Regel (z. B. Gießen alle X Tage).  
**Test-Schritte:**
1. Pflanze auswählen.
2. Pflege/Eintrag bestätigen (z. B. „Gießen“).
3. Prüfen, ob ein neuer Pflege-Eintrag erzeugt wurde (Historie).
4. Prüfen, ob „Nächste Pflege“ / Fälligkeitsdatum aktualisiert wurde.

**Erwartetes Resultat:**
- Ein neuer Eintrag erscheint in der Historie.
- Das nächste Fälligkeitsdatum verschiebt sich korrekt nach Regel-Intervall.
