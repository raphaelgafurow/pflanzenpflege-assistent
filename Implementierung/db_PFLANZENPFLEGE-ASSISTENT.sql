-- Bestehende Tabellen löschen
DROP TABLE IF EXISTS pflegeeintrag;
DROP TABLE IF EXISTS pflegeregel;
DROP TABLE IF EXISTS pflanze;

-- Tabelle: pflanze
CREATE TABLE pflanze (
    pflanzenid INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    art        VARCHAR(100),
    standort   VARCHAR(100),
    kaufdatum  DATE,
    notizen    TEXT
);

-- Tabelle: pflegeregel
CREATE TABLE pflegeregel (
    pflegeregelid INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    pflanzenid INT NOT NULL,
    typ VARCHAR(20) NOT NULL CHECK (typ IN ('Giessen', 'Duengen')),
    intervalltage INT NOT NULL CHECK (intervalltage > 0),
    startdatum DATE NOT NULL,
    CONSTRAINT fk_pflegeregel_pflanze
        FOREIGN KEY (pflanzenid)
        REFERENCES pflanze(pflanzenid)
        ON DELETE CASCADE
);

-- Tabelle: pflegeeintrag
CREATE TABLE pflegeeintrag (
    pflegeeintragid INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    pflanzenid INT NOT NULL,
    typ VARCHAR(20) NOT NULL CHECK (typ IN ('Giessen', 'Duengen')),
    datum DATE NOT NULL,
    mengeoptional VARCHAR(50),
    notizoptional TEXT,
    CONSTRAINT fk_pflegeeintrag_pflanze
        FOREIGN KEY (pflanzenid)
        REFERENCES pflanze(pflanzenid)
        ON DELETE CASCADE
);

-- Daten für Tabelle pflanze
INSERT INTO pflanze (name, art, standort, kaufdatum, notizen) VALUES
('Ficus Benjamina', 'Ficus', 'Wohnzimmer', '2024-05-10', 'mag helles, indirektes Licht'),
('Sansevieria', 'Bogenhanf', 'Schlafzimmer', '2023-09-15', 'pflegeleicht'),
('Monstera', 'Deliciosa', 'Flur', '2025-01-20', 'braucht Rankhilfe'),
('Aloe Vera', 'Aloe', 'Bad', '2022-03-02', 'wenig gießen'),
('Pilea', 'Ufopflanze', 'Küche', '2024-11-05', 'Mag viel Licht'),
('Zamioculcas', 'Glücksfeder', 'Wohnzimmer', '2021-06-10', 'sehr robust');

-- Daten für Tabelle pflegeregel
INSERT INTO pflegeregel (pflanzenid, typ, intervalltage, startdatum) VALUES
(1, 'Giessen', 7, '2025-09-01'),
(1, 'Duengen', 30, '2025-09-01'),
(2, 'Giessen', 14, '2025-09-01'),
(3, 'Giessen', 10, '2025-09-01'),
(4, 'Giessen', 7, '2025-09-01'),
(5, 'Giessen', 5, '2025-09-01'),
(6, 'Giessen', 12, '2025-09-01');

-- Daten für Tabelle pflegeeintrag
INSERT INTO pflegeeintrag (pflanzenid, typ, datum, mengeoptional, notizoptional) VALUES
(1, 'Giessen', '2025-09-12', '300 ml', 'kräftig gegossen'),
(2, 'Giessen', '2025-09-15', '200 ml', 'nur wenig Wasser'),
(3, 'Giessen', '2025-09-16', '400 ml', 'Rankhilfe kontrolliert'),
(4, 'Giessen', '2025-09-18', '150 ml', 'sparsam gießen'),
(5, 'Giessen', '2025-09-19', '250 ml', 'sehr durstig'),
(6, 'Giessen', '2025-09-20', '300 ml', 'kräftig gegossen');

-- JOIN/SELECT Abfrage
SELECT p.name, p.art, r.typ AS regel_typ, r.intervalltage, r.startdatum
FROM pflanze p
JOIN pflegeregel r ON p.pflanzenid = r.pflanzenid
ORDER BY p.name, r.typ;

