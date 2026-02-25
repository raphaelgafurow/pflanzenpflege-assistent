using System;
using System.Collections.Generic;
using System.Linq;
using PflanzenpflegeAvalonia.Models;

namespace PflanzenpflegeAvalonia.Services;

public static class PflegeStatusService
{
    public const string PflegeStatusUeberfaellig = "Überfällig";
    public const string PflegeStatusHeute = "Heute fällig";
    public const string PflegeStatusDemnaechst = "Demnächst fällig";

    public static (DateTime? Date, string Typ, string Status) ErmittlePflegeStatus(
        IEnumerable<PflegeRegel> regeln,
        IEnumerable<PflegeEintrag> eintraege,
        DateTime? heuteOverride = null)
    {
        var heute = (heuteOverride ?? DateTime.Today).Date;
        var letzteEintraegeNachTyp = eintraege
            .GroupBy(e => e.Typ)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Datum.Date));

        var kandidaten = regeln
            .Where(r => r.IntervallTage > 0)
            .Select(r =>
            {
                var letztePflege = letzteEintraegeNachTyp.TryGetValue(r.Typ, out var datum)
                    ? datum
                    : (DateTime?)null;
                var (faelligkeit, status) = BerechneFaelligkeit(r, letztePflege, heute);
                return new
                {
                    Regel = r,
                    Faelligkeit = faelligkeit,
                    Status = status
                };
            })
            .OrderBy(x => StatusPrioritaet(x.Status))
            .ThenBy(x => x.Faelligkeit)
            .ThenBy(x => x.Regel.Typ)
            .ToList();

        if (kandidaten.Count == 0)
        {
            return (null, string.Empty, "-");
        }

        var best = kandidaten[0];
        return (best.Faelligkeit, best.Regel.Typ, best.Status);
    }

    public static (DateTime Faelligkeit, string Status) BerechneFaelligkeit(
        PflegeRegel regel,
        DateTime? letztePflege,
        DateTime? heuteOverride = null)
    {
        var heute = (heuteOverride ?? DateTime.Today).Date;

        if (letztePflege is not null)
        {
            var naechsteNachPflege = letztePflege.Value.Date.AddDays(regel.IntervallTage);
            if (naechsteNachPflege > heute)
            {
                return (naechsteNachPflege, PflegeStatusDemnaechst);
            }

            if (naechsteNachPflege == heute)
            {
                return (naechsteNachPflege, PflegeStatusHeute);
            }

            return (naechsteNachPflege, PflegeStatusUeberfaellig);
        }

        var start = regel.Startdatum.Date;
        if (start > heute)
        {
            return (start, PflegeStatusDemnaechst);
        }

        var tageSeitStart = (heute - start).Days;
        var rest = tageSeitStart % regel.IntervallTage;
        var letzteFaelligkeit = heute.AddDays(-rest);

        if (rest == 0)
        {
            return (heute, PflegeStatusHeute);
        }

        return (letzteFaelligkeit, PflegeStatusUeberfaellig);
    }

    private static int StatusPrioritaet(string status)
    {
        return status switch
        {
            PflegeStatusUeberfaellig => 0,
            PflegeStatusHeute => 1,
            PflegeStatusDemnaechst => 2,
            _ => 3
        };
    }
}
