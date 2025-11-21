using System;
using System.Collections.Generic;

namespace PflanzenpflegeKonsole
{
    class Program
    {
        static List<Pflanze> pflanzen = new List<Pflanze>();

        static void Main()
        {
           
            Daten();

            Console.WriteLine("Pflanzenpflege-Assistent");
            

            int i = 1;
            foreach (var p in pflanzen)
            {
                Console.WriteLine($"{i}. {p.Name} – {p.Art} – {p.Standort} - {p.Kaufdatum} - {p.Notizen}");
                i++;
            }

            
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