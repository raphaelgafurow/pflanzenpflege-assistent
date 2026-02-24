namespace PflanzenpflegeAvalonia.Models;

public class PflanzenUebersichtItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Art { get; set; } = string.Empty;
    public string Standort { get; set; } = string.Empty;
    public string PflegeStatus { get; set; } = string.Empty;
    public string NaechstePflege { get; set; } = string.Empty;
}
