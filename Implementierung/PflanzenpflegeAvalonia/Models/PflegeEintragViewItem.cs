namespace PflanzenpflegeAvalonia.Models;

public class PflegeEintragViewItem
{
    public int Id { get; set; }
    public string Datum { get; set; } = string.Empty;
    public string Typ { get; set; } = string.Empty;
    public string Menge { get; set; } = string.Empty;
    public string Notiz { get; set; } = string.Empty;
}
