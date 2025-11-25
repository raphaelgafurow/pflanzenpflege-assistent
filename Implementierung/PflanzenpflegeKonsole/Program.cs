using System;
using System.Collections.Generic;

namespace PflanzenpflegeKonsole
{
    class Program
    {
        static List<Pflanze> pflanzen = new List<Pflanze>();

        static void Main()
        {
            Console.WriteLine("Willkommen zum Pflanzenpflege-Assistenten.");
            Console.WriteLine();
            
            Daten();

            bool weiter = true;

             while (weiter)
            {
                Console.WriteLine("=== Hauptmenü ===");
                Console.WriteLine("1) Pflanzen anzeigen");
                Console.WriteLine("2) Pflanze ändern");
                Console.WriteLine("3) Erklärung");
                Console.WriteLine("4) Beenden");
                Console.Write("Auswahl: ");

                string eingabe = Console.ReadLine();
                Console.WriteLine();

                switch (eingabe)
                {
                    case "1":
                        PflanzenAnzeigen();
                        break;

                    case "2":
                        PflanzeAendern();
                        break;

                    case "3":
                        ErklaerungAnzeigen();
                        break;

                    case "4":
                        weiter = false;
                        break;

                    default:
                        Console.WriteLine("Ungültige Eingabe.");
                        break;
                }

                Console.WriteLine();
            }

            
        }

        private static void ErklaerungAnzeigen()
        {
            Console.WriteLine("Willkommen beim Konsolen-Prototyp des Pflanzenpflege-Assistenten.");
            Console.WriteLine("In diesem Prototyp können Sie Pflanzen anzeigen und bearbeiten.");
            Console.WriteLine("Die Daten werden nur im Speicher gehalten und gehen beim Beenden verloren.");
            Console.WriteLine("Verwaltet werden pro Pflanze: Name, Art, Standort, Kaufdatum, Notizen.");
            Console.WriteLine("Bedient wird es über das Hauptmenü.");
            Console.WriteLine();
            Console.WriteLine("Weiter mit einer beliebigen Taste …");
            Console.ReadKey();
        }

         static void PflanzenAnzeigen()
        {
            Console.WriteLine("=== Pflanzenliste ===");

            if (pflanzen.Count == 0)
            {
                Console.WriteLine("Keine Pflanzen vorhanden.");
                return;
            }

            int i = 1;
            foreach (var p in pflanzen)
            {
                Console.WriteLine($"{i}. {p.Name} – {p.Art} – {p.Standort} – {p.Kaufdatum} – {p.Notizen}");
                i++;
            }
        }


         static void PflanzeAendern()
        {
            if (pflanzen.Count == 0)
            {
                Console.WriteLine("Keine Pflanzen vorhanden.");
                return;
            }

            Console.WriteLine("Welche Pflanze möchten Sie ändern?");
            PflanzenAnzeigen();

            Console.Write("Nummer eingeben: ");
            string eingabe = Console.ReadLine();

            if (!int.TryParse(eingabe, out int nummer) || nummer < 1 || nummer > pflanzen.Count)
            {
                Console.WriteLine("Ungültige Nummer.");
                return;
            }

            var p = pflanzen[nummer - 1];

            Console.WriteLine($"Ausgewählt: {p.Name}");

            Console.Write($"Neuer Name (Enter = bleibt \"{p.Name}\"): ");
            string neuerName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(neuerName))
                p.Name = neuerName;

            Console.Write($"Neue Art (Enter = bleibt \"{p.Art}\"): ");
            string neueArt = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(neueArt))
                p.Art = neueArt;

            Console.Write($"Neuer Standort (Enter = bleibt \"{p.Standort}\"): ");
            string neuerStandort = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(neuerStandort))
                p.Standort = neuerStandort;

            Console.Write($"Neues Kaufdatum (Enter = bleibt \"{p.Kaufdatum}\"): ");
            string neuesKaufdatum = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(neuesKaufdatum))
                p.Kaufdatum = neuesKaufdatum;

            Console.Write($"Neue Notizen (Enter = bleibt \"{p.Notizen}\"): ");
            string neueNotizen = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(neueNotizen))
                p.Notizen = neueNotizen;

            Console.WriteLine("Pflanze wurde aktualisiert.");
        }


        static void Daten()
        {
            pflanzen.Add(new Pflanze
            {
                Name = "Ficus Benjamina",
                Art = "Ficus",
                Standort = "Wohnzimmer",
                Kaufdatum = "2024-05-10",
                Notizen = "Mag helles, indirektes Licht"
            });

            pflanzen.Add(new Pflanze
            {
                Name = "Aloe Vera",
                Art = "Aloe",
                Standort = "Bad",
                Kaufdatum = "2022-03-02",
                Notizen = "Wenig gießen"
            });
        }
    }
}