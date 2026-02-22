using System;
using System.ComponentModel.DataAnnotations;

namespace PflanzenpflegeAvalonia.Models;

public class PflegeEintrag
{
    public int Id { get; set; }

    public int PflanzeId { get; set; }
    public Pflanze Pflanze { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Typ { get; set; } = string.Empty;

    public DateTime Datum { get; set; }

    [MaxLength(100)]
    public string Menge { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Notiz { get; set; } = string.Empty;
}
