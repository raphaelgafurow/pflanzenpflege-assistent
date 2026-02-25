using System;
using System.Collections.Generic;
using PflanzenpflegeAvalonia.Models;
using PflanzenpflegeAvalonia.Services;
using Xunit;

namespace PflanzenpflegeAvalonia.Tests;

public class PflegeStatusServiceTests
{
    [Fact]
    public void BerechneFaelligkeit_WithoutLastCare_AndFutureStart_ReturnsDemnaechst()
    {
        var regel = new PflegeRegel { IntervallTage = 7, Startdatum = new DateTime(2026, 3, 1) };

        var result = PflegeStatusService.BerechneFaelligkeit(
            regel,
            letztePflege: null,
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Equal(new DateTime(2026, 3, 1), result.Faelligkeit);
        Assert.Equal(PflegeStatusService.PflegeStatusDemnaechst, result.Status);
    }

    [Fact]
    public void BerechneFaelligkeit_WithoutLastCare_AndStartToday_ReturnsHeute()
    {
        var heute = new DateTime(2026, 2, 25);
        var regel = new PflegeRegel { IntervallTage = 5, Startdatum = heute };

        var result = PflegeStatusService.BerechneFaelligkeit(regel, null, heute);

        Assert.Equal(heute, result.Faelligkeit);
        Assert.Equal(PflegeStatusService.PflegeStatusHeute, result.Status);
    }

    [Fact]
    public void BerechneFaelligkeit_WithoutLastCare_AndPastStart_ReturnsUeberfaellig()
    {
        var regel = new PflegeRegel { IntervallTage = 7, Startdatum = new DateTime(2026, 2, 1) };

        var result = PflegeStatusService.BerechneFaelligkeit(
            regel,
            letztePflege: null,
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Equal(new DateTime(2026, 2, 22), result.Faelligkeit);
        Assert.Equal(PflegeStatusService.PflegeStatusUeberfaellig, result.Status);
    }

    [Fact]
    public void BerechneFaelligkeit_WithLastCareInPast_AndNotDueYet_ReturnsDemnaechst()
    {
        var regel = new PflegeRegel { IntervallTage = 7, Startdatum = new DateTime(2026, 1, 1) };

        var result = PflegeStatusService.BerechneFaelligkeit(
            regel,
            letztePflege: new DateTime(2026, 2, 20),
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Equal(new DateTime(2026, 2, 27), result.Faelligkeit);
        Assert.Equal(PflegeStatusService.PflegeStatusDemnaechst, result.Status);
    }

    [Fact]
    public void BerechneFaelligkeit_WithLastCare_AndDueToday_ReturnsHeute()
    {
        var regel = new PflegeRegel { IntervallTage = 7, Startdatum = new DateTime(2026, 1, 1) };

        var result = PflegeStatusService.BerechneFaelligkeit(
            regel,
            letztePflege: new DateTime(2026, 2, 18),
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Equal(new DateTime(2026, 2, 25), result.Faelligkeit);
        Assert.Equal(PflegeStatusService.PflegeStatusHeute, result.Status);
    }

    [Fact]
    public void ErmittlePflegeStatus_PrioritizesUeberfaelligOverDemnaechst()
    {
        var regeln = new List<PflegeRegel>
        {
            new() { Typ = "Giessen", IntervallTage = 7, Startdatum = new DateTime(2026, 2, 1) },
            new() { Typ = "Duengen", IntervallTage = 14, Startdatum = new DateTime(2026, 3, 5) }
        };

        var eintraege = new List<PflegeEintrag>();

        var result = PflegeStatusService.ErmittlePflegeStatus(
            regeln,
            eintraege,
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Equal("Giessen", result.Typ);
        Assert.Equal(PflegeStatusService.PflegeStatusUeberfaellig, result.Status);
    }

    [Fact]
    public void ErmittlePflegeStatus_WithNoValidRules_ReturnsFallback()
    {
        var regeln = new List<PflegeRegel>
        {
            new() { Typ = "Giessen", IntervallTage = 0, Startdatum = new DateTime(2026, 2, 1) }
        };

        var result = PflegeStatusService.ErmittlePflegeStatus(
            regeln,
            eintraege: Array.Empty<PflegeEintrag>(),
            heuteOverride: new DateTime(2026, 2, 25));

        Assert.Null(result.Date);
        Assert.Equal(string.Empty, result.Typ);
        Assert.Equal("-", result.Status);
    }
}
