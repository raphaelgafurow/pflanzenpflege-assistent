using System;
using System.ComponentModel.DataAnnotations;

namespace PflanzenpflegeAvalonia.Models;

public class PflegeRegel
{
    public int Id { get; set; }

    public int PflanzeId { get; set; }
    public Pflanze Pflanze { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Typ { get; set; } = string.Empty;

    public int IntervallTage { get; set; }
    public DateTime Startdatum { get; set; }
}
