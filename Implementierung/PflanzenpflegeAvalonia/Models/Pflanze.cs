using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PflanzenpflegeAvalonia.Models;

public class Pflanze
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Art { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Standort { get; set; } = string.Empty;

    public DateTime Kaufdatum { get; set; }

    [MaxLength(500)]
    public string Notizen { get; set; } = string.Empty;

    public List<PflegeRegel> Regeln { get; set; } = new();
    public List<PflegeEintrag> Eintraege { get; set; } = new();
}
