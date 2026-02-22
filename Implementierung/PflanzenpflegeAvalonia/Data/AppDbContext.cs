using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PflanzenpflegeAvalonia.Models;

namespace PflanzenpflegeAvalonia.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pflanze> Pflanzen => Set<Pflanze>();
    public DbSet<PflegeRegel> PflegeRegeln => Set<PflegeRegel>();
    public DbSet<PflegeEintrag> PflegeEintraege => Set<PflegeEintrag>();

    public static string GetDatabasePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folder = Path.Combine(appData, "PflanzenpflegeAssistent");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "pflanzenpflege.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = GetDatabasePath();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pflanze>()
            .HasIndex(x => new { x.Name, x.Standort })
            .IsUnique();

        modelBuilder.Entity<PflegeRegel>()
            .HasOne(x => x.Pflanze)
            .WithMany(x => x.Regeln)
            .HasForeignKey(x => x.PflanzeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PflegeEintrag>()
            .HasOne(x => x.Pflanze)
            .WithMany(x => x.Eintraege)
            .HasForeignKey(x => x.PflanzeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public static void EnsureCreatedAndSeeded()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.Database.ExecuteSqlRaw(
            @"CREATE TABLE IF NOT EXISTS PflegeRegeln (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PflanzeId INTEGER NOT NULL,
                Typ TEXT NOT NULL,
                IntervallTage INTEGER NOT NULL,
                Startdatum TEXT NOT NULL,
                FOREIGN KEY (PflanzeId) REFERENCES Pflanzen (Id) ON DELETE CASCADE
            );");
        db.Database.ExecuteSqlRaw(
            @"CREATE TABLE IF NOT EXISTS PflegeEintraege (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PflanzeId INTEGER NOT NULL,
                Typ TEXT NOT NULL,
                Datum TEXT NOT NULL,
                Menge TEXT NOT NULL DEFAULT '',
                Notiz TEXT NOT NULL DEFAULT '',
                FOREIGN KEY (PflanzeId) REFERENCES Pflanzen (Id) ON DELETE CASCADE
            );");
        db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_PflegeRegeln_PflanzeId ON PflegeRegeln(PflanzeId);");
        db.Database.ExecuteSqlRaw("CREATE INDEX IF NOT EXISTS IX_PflegeEintraege_PflanzeId ON PflegeEintraege(PflanzeId);");

        SeedBeispielPflanzen(db);
        SeedBeispielRegelnUndEintraege(db);
    }

    private static void SeedBeispielPflanzen(AppDbContext db)
    {
        var beispiele = new List<Pflanze>
        {
            new Pflanze
            {
                Name = "Ficus Benjamina",
                Art = "Ficus",
                Standort = "Wohnzimmer",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Hell und ohne direkte Mittagssonne."
            },
            new Pflanze
            {
                Name = "Sanseviera",
                Art = "Bogenhanf",
                Standort = "Schlafzimmer",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Sehr pflegeleicht, lieber zu trocken als zu nass."
            },
            new Pflanze
            {
                Name = "Aloe Vera",
                Art = "Aloe",
                Standort = "Bad",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Nur sparsam gießen."
            },
            new Pflanze
            {
                Name = "Monstera",
                Art = "Deliciosa",
                Standort = "Flur",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Regelmäßig Blätter abwischen."
            },
            new Pflanze
            {
                Name = "Pilea",
                Art = "Ufopflanze",
                Standort = "Küche",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Drehen für gleichmäßigen Wuchs."
            },
            new Pflanze
            {
                Name = "Zamioculcas",
                Art = "Glücksfeder",
                Standort = "Wohnzimmer",
                Kaufdatum = new DateTime(2025, 9, 1),
                Notizen = "Kommt auch mit wenig Licht klar."
            },
            new Pflanze
            {
                Name = "Calathea Orbifolia",
                Art = "Calathea",
                Standort = "Arbeitszimmer",
                Kaufdatum = new DateTime(2025, 11, 3),
                Notizen = "Hohe Luftfeuchtigkeit."
            },
            new Pflanze
            {
                Name = "Spathiphyllum",
                Art = "Einblatt",
                Standort = "Wohnzimmer",
                Kaufdatum = new DateTime(2025, 10, 18),
                Notizen = "Leicht feucht halten."
            },
            new Pflanze
            {
                Name = "Rosmarin",
                Art = "Kräuterpflanze",
                Standort = "Balkon",
                Kaufdatum = new DateTime(2025, 7, 21),
                Notizen = "Viel Sonne."
            },
            new Pflanze
            {
                Name = "Farn Boston",
                Art = "Nephrolepis",
                Standort = "Bad",
                Kaufdatum = new DateTime(2025, 12, 2),
                Notizen = "Regelmäßig besprühen."
            },
            new Pflanze
            {
                Name = "Orchidee Luna",
                Art = "Phalaenopsis",
                Standort = "Fensterbank",
                Kaufdatum = new DateTime(2026, 1, 10),
                Notizen = "Einmal pro Woche tauchen."
            },
            new Pflanze
            {
                Name = "Bonsai Kenji",
                Art = "Ficus Ginseng",
                Standort = "Schreibtisch",
                Kaufdatum = new DateTime(2025, 8, 6),
                Notizen = "Kleine Mengen, aber regelmäßig."
            }
        };

        foreach (var item in beispiele)
        {
            var vorhanden = db.Pflanzen.FirstOrDefault(p => p.Name == item.Name && p.Standort == item.Standort);
            if (vorhanden is null)
            {
                db.Pflanzen.Add(item);
            }
            else
            {
                vorhanden.Art = item.Art;
                vorhanden.Kaufdatum = item.Kaufdatum;
                vorhanden.Standort = item.Standort;
                if (string.IsNullOrWhiteSpace(vorhanden.Notizen))
                {
                    vorhanden.Notizen = item.Notizen;
                }
            }
        }

        db.SaveChanges();
    }

    private static void SeedBeispielRegelnUndEintraege(AppDbContext db)
    {
        SeedRegelnFuerPflanze(
            db,
            "Ficus Benjamina",
            "Wohnzimmer",
            ("Giessen", 7, new DateTime(2025, 9, 1)),
            ("Düngen", 30, new DateTime(2025, 9, 1)));

        SeedRegelnFuerPflanze(
            db,
            "Sanseviera",
            "Schlafzimmer",
            ("Giessen", 14, new DateTime(2025, 9, 5)),
            ("Düngen", 45, new DateTime(2025, 10, 1)));

        SeedRegelnFuerPflanze(
            db,
            "Aloe Vera",
            "Bad",
            ("Giessen", 12, new DateTime(2025, 9, 2)),
            ("Düngen", 40, new DateTime(2025, 10, 2)));

        SeedRegelnFuerPflanze(
            db,
            "Monstera",
            "Flur",
            ("Giessen", 8, new DateTime(2025, 9, 3)),
            ("Düngen", 28, new DateTime(2025, 9, 15)));

        SeedRegelnFuerPflanze(
            db,
            "Pilea",
            "Küche",
            ("Giessen", 7, new DateTime(2025, 9, 4)),
            ("Düngen", 30, new DateTime(2025, 9, 20)));

        SeedRegelnFuerPflanze(
            db,
            "Zamioculcas",
            "Wohnzimmer",
            ("Giessen", 18, new DateTime(2025, 9, 8)),
            ("Düngen", 50, new DateTime(2025, 10, 8)));

        SeedRegelnFuerPflanze(
            db,
            "Calathea Orbifolia",
            "Arbeitszimmer",
            ("Giessen", 6, new DateTime(2025, 11, 3)),
            ("Düngen", 21, new DateTime(2025, 11, 10)));

        SeedRegelnFuerPflanze(
            db,
            "Spathiphyllum",
            "Wohnzimmer",
            ("Giessen", 5, new DateTime(2025, 10, 18)),
            ("Düngen", 30, new DateTime(2025, 11, 1)));

        SeedRegelnFuerPflanze(
            db,
            "Rosmarin",
            "Balkon",
            ("Giessen", 4, new DateTime(2025, 7, 21)),
            ("Düngen", 20, new DateTime(2025, 8, 1)));

        SeedRegelnFuerPflanze(
            db,
            "Farn Boston",
            "Bad",
            ("Giessen", 3, new DateTime(2025, 12, 2)),
            ("Düngen", 18, new DateTime(2025, 12, 20)));

        SeedRegelnFuerPflanze(
            db,
            "Orchidee Luna",
            "Fensterbank",
            ("Giessen", 7, new DateTime(2026, 1, 10)),
            ("Düngen", 35, new DateTime(2026, 1, 20)));

        SeedRegelnFuerPflanze(
            db,
            "Bonsai Kenji",
            "Schreibtisch",
            ("Giessen", 2, new DateTime(2025, 8, 6)),
            ("Düngen", 14, new DateTime(2025, 8, 20)));

        SeedEintraegeFuerPflanze(
            db,
            "Ficus Benjamina",
            "Wohnzimmer",
            (new DateTime(2025, 9, 1), "Giessen", "300 ml", "kräftig gegossen"),
            (new DateTime(2025, 9, 7), "Giessen", "200 ml", "nur wenig Wasser"),
            (new DateTime(2025, 10, 1), "Düngen", "5 ml", "Flüssigdünger"),
            (new DateTime(2026, 1, 20), "Giessen", "250 ml", "Erde war trocken"));

        SeedEintraegeFuerPflanze(
            db,
            "Sanseviera",
            "Schlafzimmer",
            (new DateTime(2025, 9, 5), "Giessen", "150 ml", "sparsam"),
            (new DateTime(2025, 10, 3), "Giessen", "120 ml", "nur leicht"),
            (new DateTime(2025, 11, 15), "Düngen", "2 ml", "Sukkulenten-Dünger"));

        SeedEintraegeFuerPflanze(
            db,
            "Aloe Vera",
            "Bad",
            (new DateTime(2025, 9, 2), "Giessen", "100 ml", "Substrat trocken"),
            (new DateTime(2025, 10, 15), "Giessen", "120 ml", "wenig Wasser"),
            (new DateTime(2026, 1, 18), "Düngen", "3 ml", "leicht gedüngt"));

        SeedEintraegeFuerPflanze(
            db,
            "Monstera",
            "Flur",
            (new DateTime(2025, 9, 3), "Giessen", "350 ml", "gleichmäßig"),
            (new DateTime(2025, 9, 15), "Düngen", "6 ml", "Wachstumsdünger"),
            (new DateTime(2026, 2, 10), "Giessen", "300 ml", "letzte Pflege"));

        SeedEintraegeFuerPflanze(
            db,
            "Pilea",
            "Küche",
            (new DateTime(2025, 9, 4), "Giessen", "180 ml", "normal"),
            (new DateTime(2025, 10, 4), "Düngen", "4 ml", "leicht dosiert"),
            (new DateTime(2026, 2, 14), "Giessen", "170 ml", ""));

        SeedEintraegeFuerPflanze(
            db,
            "Zamioculcas",
            "Wohnzimmer",
            (new DateTime(2025, 9, 8), "Giessen", "120 ml", "sehr wenig"),
            (new DateTime(2025, 11, 1), "Giessen", "140 ml", "Erde trocken"),
            (new DateTime(2026, 1, 5), "Düngen", "2 ml", "schwach"));

        SeedEintraegeFuerPflanze(
            db,
            "Calathea Orbifolia",
            "Arbeitszimmer",
            (new DateTime(2025, 11, 3), "Giessen", "220 ml", "entkalktes Wasser"),
            (new DateTime(2025, 11, 10), "Düngen", "3 ml", "leicht"),
            (new DateTime(2026, 2, 11), "Giessen", "200 ml", "Blätter besprüht"));

        SeedEintraegeFuerPflanze(
            db,
            "Spathiphyllum",
            "Wohnzimmer",
            (new DateTime(2025, 10, 18), "Giessen", "250 ml", "hatte hängende Blätter"),
            (new DateTime(2025, 11, 1), "Düngen", "5 ml", "Blütedünger"),
            (new DateTime(2026, 2, 16), "Giessen", "230 ml", "normal"));

        SeedEintraegeFuerPflanze(
            db,
            "Rosmarin",
            "Balkon",
            (new DateTime(2025, 8, 1), "Düngen", "4 ml", "Kräuterdünger"),
            (new DateTime(2025, 12, 10), "Giessen", "200 ml", "Winterpflege"),
            (new DateTime(2026, 2, 5), "Giessen", "180 ml", "kühles Wetter"));

        SeedEintraegeFuerPflanze(
            db,
            "Farn Boston",
            "Bad",
            (new DateTime(2025, 12, 2), "Giessen", "260 ml", "Substrat feucht"),
            (new DateTime(2025, 12, 20), "Düngen", "4 ml", "Farn-Dünger"),
            (new DateTime(2026, 2, 20), "Giessen", "240 ml", "sehr trocken"));

        SeedEintraegeFuerPflanze(
            db,
            "Orchidee Luna",
            "Fensterbank",
            (new DateTime(2026, 1, 10), "Giessen", "Tauchen", "10 Minuten getaucht"),
            (new DateTime(2026, 1, 20), "Düngen", "2 ml", "Orchideen-Dünger"),
            (new DateTime(2026, 2, 17), "Giessen", "Tauchen", "kurz getaucht"));

        SeedEintraegeFuerPflanze(
            db,
            "Bonsai Kenji",
            "Schreibtisch",
            (new DateTime(2025, 8, 6), "Giessen", "80 ml", "kleine Menge"),
            (new DateTime(2025, 8, 20), "Düngen", "1 ml", "Bonsai-Dünger"),
            (new DateTime(2026, 2, 21), "Giessen", "70 ml", ""));
    }

    private static void SeedRegelnFuerPflanze(
        AppDbContext db,
        string pflanzenName,
        string standort,
        params (string Typ, int IntervallTage, DateTime Startdatum)[] regeln)
    {
        var pflanze = db.Pflanzen.FirstOrDefault(p => p.Name == pflanzenName && p.Standort == standort);
        if (pflanze is null)
        {
            return;
        }

        foreach (var daten in regeln)
        {
            var regel = db.PflegeRegeln.FirstOrDefault(r => r.PflanzeId == pflanze.Id && r.Typ == daten.Typ);
            if (regel is null)
            {
                db.PflegeRegeln.Add(new PflegeRegel
                {
                    PflanzeId = pflanze.Id,
                    Typ = daten.Typ,
                    IntervallTage = daten.IntervallTage,
                    Startdatum = daten.Startdatum
                });
            }
            else
            {
                regel.IntervallTage = daten.IntervallTage;
                regel.Startdatum = daten.Startdatum;
            }
        }

        db.SaveChanges();
    }

    private static void SeedEintraegeFuerPflanze(
        AppDbContext db,
        string pflanzenName,
        string standort,
        params (DateTime Datum, string Typ, string Menge, string Notiz)[] eintraege)
    {
        var pflanze = db.Pflanzen.FirstOrDefault(p => p.Name == pflanzenName && p.Standort == standort);
        if (pflanze is null)
        {
            return;
        }

        foreach (var daten in eintraege)
        {
            var vorhanden = db.PflegeEintraege.Any(e =>
                e.PflanzeId == pflanze.Id &&
                e.Datum.Date == daten.Datum.Date &&
                e.Typ == daten.Typ &&
                e.Notiz == daten.Notiz);

            if (vorhanden)
            {
                continue;
            }

            db.PflegeEintraege.Add(new PflegeEintrag
            {
                PflanzeId = pflanze.Id,
                Datum = daten.Datum,
                Typ = daten.Typ,
                Menge = daten.Menge,
                Notiz = daten.Notiz
            });
        }

        db.SaveChanges();
    }
}
