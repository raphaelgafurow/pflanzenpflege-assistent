using System;

namespace PflanzenpflegeAvalonia.Models
{
    public class Pflanze
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Art { get; set; } = "";

        public string Standort { get; set; } = "";

        public DateTime Kaufdatum { get; set; }

        public string Notizen { get; set; } = "";
    }
}
