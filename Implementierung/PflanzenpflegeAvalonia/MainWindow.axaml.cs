using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using PflanzenpflegeAvalonia.Data;
using PflanzenpflegeAvalonia.Models;
using PflanzenpflegeAvalonia.Services;

namespace PflanzenpflegeAvalonia;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<PflanzenUebersichtItem> _pflanzen = new();
    private readonly ObservableCollection<RegelViewItem> _regeln = new();
    private readonly ObservableCollection<PflegeEintragViewItem> _eintraege = new();
    private PflanzenUebersichtItem? _ausgewaehltePflanze;
    private int? _pflanzeIdZumBearbeiten;
    private int? _regelIdZumBearbeiten;

    public MainWindow()
    {
        InitializeComponent();

        PflanzenDataGrid.ItemsSource = _pflanzen;
        RegelnDataGrid.ItemsSource = _regeln;
        TabEintraegeDataGrid.ItemsSource = _eintraege;

        TabRegelTypComboBox.ItemsSource = new[] { "Giessen", "Düngen", "Umtopfen" };
        TabRegelTypComboBox.SelectedIndex = 0;
        TabRegelStartdatumPicker.SelectedDate = DateTimeOffset.Now;

        TabEintragTypComboBox.ItemsSource = new[] { "Giessen", "Düngen", "Umtopfen" };
        TabEintragTypComboBox.SelectedIndex = 0;
        TabEintragDatumPicker.SelectedDate = DateTimeOffset.Now;
        TabKaufdatumPicker.SelectedDate = DateTimeOffset.Now;

        PflanzenDataGrid.SelectionChanged += (_, _) => PflanzeAusgewaehlt();
        RegelnDataGrid.SelectionChanged += (_, _) => LadeRegelInFormular();
        TabEintraegeDataGrid.SelectionChanged += (_, _) => LadeEintragInFormular();

        BtnPflanzeAnlegen.Click += (_, _) => OeffnePflanzeTabNeu();
        BtnPflanzeBearbeiten.Click += (_, _) => OeffnePflanzeTabBearbeiten();
        BtnPflanzeLoeschen.Click += async (_, _) => await PflanzeLoeschen();
        BtnRegelBearbeiten.Click += (_, _) => OeffneRegelTab();
        BtnEintraegeBearbeiten.Click += (_, _) => OeffneEintraegeTab();
        BtnPflegeBestaetigen.Click += async (_, _) => await PflegeBestaetigen();
        BtnBeenden.Click += (_, _) => Close();
        BtnRegelLoeschen.Click += async (_, _) => await RegelLoeschen();

        MenuUebersicht.Click += (_, _) => MainTabs.SelectedIndex = 0;
        MenuPflanzeAnlegen.Click += (_, _) => OeffnePflanzeTabNeu();
        MenuPflanzeBearbeiten.Click += (_, _) => OeffnePflanzeTabBearbeiten();
        MenuPflanzeLoeschen.Click += async (_, _) => await PflanzeLoeschen();
        MenuRegelBearbeiten.Click += (_, _) => OeffneRegelTab();
        MenuEintraege.Click += (_, _) => OeffneEintraegeTab();
        MenuPflegeBestaetigen.Click += async (_, _) => await PflegeBestaetigen();
        MenuBeenden.Click += (_, _) => Close();
        MenuAktualisieren.Click += (_, _) => LadeDaten();

        SucheButton.Click += (_, _) => LadeDaten();
        SucheZuruecksetzenButton.Click += (_, _) =>
        {
            SuchTextBox.Text = string.Empty;
            LadeDaten();
        };

        TabPflanzeSpeichernButton.Click += (_, _) => PflanzeAusTabSpeichern();
        TabPflanzeZuruecksetzenButton.Click += (_, _) => PflanzeTabZuruecksetzen();
        TabRegelSpeichernButton.Click += (_, _) => RegelAusTabSpeichern();
        TabRegelNeuButton.Click += (_, _) => RegelTabZuruecksetzen();

        TabEintragHinzufuegenButton.Click += (_, _) => EintragAusTabHinzufuegen();
        TabEintragAendernButton.Click += (_, _) => EintragAusTabAendern();
        TabEintragLoeschenButton.Click += async (_, _) => await EintragAusTabLoeschen();
        TabEintragZuruecksetzenButton.Click += (_, _) => EintragTabZuruecksetzen();

        LadeDaten();
    }

    private void LadeDaten()
    {
        _pflanzen.Clear();

        using var db = new AppDbContext();
        var suche = (SuchTextBox.Text ?? string.Empty).Trim();

        var query = db.Pflanzen
            .Include(x => x.Regeln)
            .Include(x => x.Eintraege)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(suche))
        {
            var sucheKlein = suche.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(sucheKlein) ||
                p.Art.ToLower().Contains(sucheKlein) ||
                p.Standort.ToLower().Contains(sucheKlein));
        }

        var pflanzen = query
            .OrderBy(x => x.Name)
            .ToList();

        foreach (var p in pflanzen)
        {
            var status = PflegeStatusService.ErmittlePflegeStatus(p.Regeln, p.Eintraege);
            var nextText = status.Date is null ? "-" : $"{status.Date:dd.MM.yyyy} ({status.Typ})";

            _pflanzen.Add(new PflanzenUebersichtItem
            {
                Id = p.Id,
                Name = p.Name,
                Art = p.Art,
                Standort = p.Standort,
                PflegeStatus = status.Status,
                NaechstePflege = nextText
            });
        }

        _ausgewaehltePflanze = null;
        _pflanzeIdZumBearbeiten = null;
        _regelIdZumBearbeiten = null;
        _regeln.Clear();
        _eintraege.Clear();
        AusgewaehltText.Text = "Ausgewählt: -";
        NaechstePflegeText.Text = "-";
        TabRegelNameText.Text = "Bitte in Übersicht eine Pflanze wählen";
        TabEintraegeNameText.Text = "Bitte in Übersicht eine Pflanze wählen";
        if (string.IsNullOrWhiteSpace(suche))
        {
            StatusSetzen($"Übersicht aktualisiert ({_pflanzen.Count} Pflanzen).");
        }
        else
        {
            StatusSetzen($"Suche \"{suche}\": {_pflanzen.Count} Treffer.");
        }
    }

    private void PflanzeAusgewaehlt()
    {
        _ausgewaehltePflanze = PflanzenDataGrid.SelectedItem as PflanzenUebersichtItem;
        if (_ausgewaehltePflanze is null)
        {
            AusgewaehltText.Text = "Ausgewählt: -";
            NaechstePflegeText.Text = "-";
            _regeln.Clear();
            _eintraege.Clear();
            _regelIdZumBearbeiten = null;
            return;
        }

        AusgewaehltText.Text = $"Ausgewählt: {_ausgewaehltePflanze.Name} - {_ausgewaehltePflanze.Standort}";
        TabRegelNameText.Text = $"{_ausgewaehltePflanze.Name} - {_ausgewaehltePflanze.Standort}";
        TabEintraegeNameText.Text = $"{_ausgewaehltePflanze.Name} - {_ausgewaehltePflanze.Standort}";

        LadeRegelnFuerAuswahl();
        LadeEintraegeFuerAuswahl();
    }

    private void LadeRegelnFuerAuswahl()
    {
        _regeln.Clear();
        if (_ausgewaehltePflanze is null)
        {
            return;
        }

        using var db = new AppDbContext();
        var regeln = db.PflegeRegeln
            .Where(r => r.PflanzeId == _ausgewaehltePflanze.Id)
            .OrderBy(r => r.Typ)
            .ToList();
        var eintraege = db.PflegeEintraege
            .Where(e => e.PflanzeId == _ausgewaehltePflanze.Id)
            .ToList();

        foreach (var regel in regeln)
        {
            _regeln.Add(new RegelViewItem
            {
                Id = regel.Id,
                Typ = regel.Typ,
                Intervall = $"{regel.IntervallTage} Tage",
                Startdatum = regel.Startdatum.ToString("dd.MM.yyyy")
            });
        }

        var status = PflegeStatusService.ErmittlePflegeStatus(regeln, eintraege);
        NaechstePflegeText.Text = status.Date is null ? "-" : $"{status.Date:dd.MM.yyyy}\n{status.Typ}\n{status.Status}";
    }

    private void LadeEintraegeFuerAuswahl()
    {
        _eintraege.Clear();
        if (_ausgewaehltePflanze is null)
        {
            return;
        }

        using var db = new AppDbContext();
        var rows = db.PflegeEintraege
            .Where(x => x.PflanzeId == _ausgewaehltePflanze.Id)
            .OrderByDescending(x => x.Datum)
            .ToList();

        foreach (var row in rows)
        {
            _eintraege.Add(new PflegeEintragViewItem
            {
                Id = row.Id,
                Datum = row.Datum.ToString("dd.MM.yyyy"),
                Typ = row.Typ,
                Menge = row.Menge,
                Notiz = row.Notiz
            });
        }
    }

    private async System.Threading.Tasks.Task PflanzeLoeschen()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst eine Pflanze auswählen.");
            return;
        }

        var bestaetigt = await BestaetigungsDialog(
            "Löschen bestätigen",
            $"Möchtest du die Pflanze \"{_ausgewaehltePflanze.Name}\" wirklich löschen?",
            "Ja, löschen");
        if (!bestaetigt)
        {
            StatusSetzen("Löschen abgebrochen.");
            return;
        }

        using var db = new AppDbContext();
        var pflanze = db.Pflanzen.FirstOrDefault(x => x.Id == _ausgewaehltePflanze.Id);
        if (pflanze is null)
        {
            StatusSetzen("Pflanze wurde nicht gefunden.");
            return;
        }

        db.Pflanzen.Remove(pflanze);
        db.SaveChanges();
        LadeDaten();
        StatusSetzen("Pflanze wurde gelöscht.");
    }

    private async System.Threading.Tasks.Task RegelLoeschen()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst eine Pflanze auswählen.");
            return;
        }

        if (RegelnDataGrid.SelectedItem is not RegelViewItem regel)
        {
            StatusSetzen("Bitte zuerst eine Regel auswählen.");
            return;
        }

        var bestaetigt = await BestaetigungsDialog(
            "Löschen bestätigen",
            $"Möchtest du die Regel \"{regel.Typ}\" wirklich löschen?",
            "Ja, löschen");
        if (!bestaetigt)
        {
            StatusSetzen("Löschen abgebrochen.");
            return;
        }

        using var db = new AppDbContext();
        var entity = db.PflegeRegeln.FirstOrDefault(x => x.Id == regel.Id && x.PflanzeId == _ausgewaehltePflanze.Id);
        if (entity is null)
        {
            StatusSetzen("Regel wurde nicht gefunden.");
            return;
        }

        db.PflegeRegeln.Remove(entity);
        db.SaveChanges();

        LadeRegelnFuerAuswahl();
        RegelTabZuruecksetzen();
        StatusSetzen("Regel wurde gelöscht.");
    }

    private async System.Threading.Tasks.Task PflegeBestaetigen()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst eine Pflanze auswählen.");
            return;
        }

        if (RegelnDataGrid.SelectedItem is not RegelViewItem ausgewaehlteRegel)
        {
            StatusSetzen("Bitte unten zuerst eine Regel auswählen und dann Pflege bestätigen.");
            return;
        }

        using var db = new AppDbContext();
        var regel = db.PflegeRegeln.FirstOrDefault(r =>
            r.Id == ausgewaehlteRegel.Id &&
            r.PflanzeId == _ausgewaehltePflanze.Id &&
            r.IntervallTage > 0);
        if (regel is null)
        {
            StatusSetzen("Die ausgewählte Regel ist ungültig oder wurde nicht gefunden.");
            return;
        }

        var heute = DateTime.Today;
        var letztePflege = db.PflegeEintraege
            .Where(e => e.PflanzeId == _ausgewaehltePflanze.Id && e.Typ == regel.Typ)
            .OrderByDescending(e => e.Datum)
            .Select(e => (DateTime?)e.Datum)
            .FirstOrDefault();
        var (faelligkeit, status) = PflegeStatusService.BerechneFaelligkeit(regel, letztePflege, heute);
        if (status == PflegeStatusService.PflegeStatusDemnaechst)
        {
            StatusSetzen($"Die ausgewählte Regel ist erst am {faelligkeit:dd.MM.yyyy} fällig.");
            return;
        }

        var bestaetigt = await BestaetigungsDialog(
            "Pflege bestätigen",
            $"Pflege \"{regel.Typ}\" für \"{_ausgewaehltePflanze.Name}\" am {heute:dd.MM.yyyy} als erledigt bestätigen? (Fällig seit {faelligkeit:dd.MM.yyyy})",
            "Ja, bestätigen");
        if (!bestaetigt)
        {
            StatusSetzen("Bestätigung abgebrochen.");
            return;
        }

        db.PflegeEintraege.Add(new PflegeEintrag
        {
            PflanzeId = _ausgewaehltePflanze.Id,
            Typ = regel.Typ,
            Datum = heute,
            Menge = string.Empty,
            Notiz = "Als erledigt bestätigt"
        });

        db.SaveChanges();

        var pflanzeId = _ausgewaehltePflanze.Id;
        LadeDaten();
        var erneutAuswaehlen = _pflanzen.FirstOrDefault(x => x.Id == pflanzeId);
        if (erneutAuswaehlen is not null)
        {
            PflanzenDataGrid.SelectedItem = erneutAuswaehlen;
        }

        StatusSetzen($"Pflege bestätigt: {regel.Typ}. Nächste Fälligkeit wurde verschoben.");
    }

    private void OeffneRegelTab()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst in Übersicht eine Pflanze auswählen.");
            MainTabs.SelectedIndex = 0;
            return;
        }

        MainTabs.SelectedIndex = 2;
    }

    private void OeffneEintraegeTab()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst in Übersicht eine Pflanze auswählen.");
            MainTabs.SelectedIndex = 0;
            return;
        }

        MainTabs.SelectedIndex = 3;
    }

    private void OeffnePflanzeTabNeu()
    {
        _pflanzeIdZumBearbeiten = null;
        PflanzeTabZuruecksetzen();
        MainTabs.SelectedIndex = 1;
    }

    private void OeffnePflanzeTabBearbeiten()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst in Übersicht eine Pflanze auswählen.");
            MainTabs.SelectedIndex = 0;
            return;
        }

        using var db = new AppDbContext();
        var pflanze = db.Pflanzen.FirstOrDefault(x => x.Id == _ausgewaehltePflanze.Id);
        if (pflanze is null)
        {
            StatusSetzen("Pflanze wurde nicht gefunden.");
            return;
        }

        _pflanzeIdZumBearbeiten = pflanze.Id;
        TabNameBox.Text = pflanze.Name;
        TabArtBox.Text = pflanze.Art;
        TabStandortBox.Text = pflanze.Standort;
        TabNotizenBox.Text = pflanze.Notizen;
        TabKaufdatumPicker.SelectedDate = new DateTimeOffset(pflanze.Kaufdatum);
        MainTabs.SelectedIndex = 1;
    }

    private void PflanzeAusTabSpeichern()
    {
        var name = (TabNameBox.Text ?? string.Empty).Trim();
        var art = (TabArtBox.Text ?? string.Empty).Trim();
        var standort = (TabStandortBox.Text ?? string.Empty).Trim();
        var notizen = (TabNotizenBox.Text ?? string.Empty).Trim();
        var kaufdatum = TabKaufdatumPicker.SelectedDate;

        var validierungsfehler = ValidierePflanze(name, art, standort, kaufdatum);
        if (validierungsfehler is not null)
        {
            StatusSetzen(validierungsfehler);
            return;
        }

        using var db = new AppDbContext();
        Pflanze pflanze;
        if (_pflanzeIdZumBearbeiten is null)
        {
            if (IstNameStandortDuplikat(db.Pflanzen, name, standort))
            {
                StatusSetzen("Eine Pflanze mit diesem Namen und Standort existiert bereits.");
                return;
            }

            pflanze = new Pflanze();
            db.Pflanzen.Add(pflanze);
        }
        else
        {
            pflanze = db.Pflanzen.FirstOrDefault(x => x.Id == _pflanzeIdZumBearbeiten.Value)
                ?? new Pflanze();
            if (pflanze.Id == 0)
            {
                StatusSetzen("Pflanze wurde nicht gefunden.");
                return;
            }

            if (IstNameStandortDuplikat(db.Pflanzen, name, standort, pflanze.Id))
            {
                StatusSetzen("Eine andere Pflanze mit diesem Namen und Standort existiert bereits.");
                return;
            }
        }

        pflanze.Name = name;
        pflanze.Art = art;
        pflanze.Standort = standort;
        pflanze.Kaufdatum = kaufdatum!.Value.DateTime.Date;
        pflanze.Notizen = notizen;

        var warBearbeiten = _pflanzeIdZumBearbeiten.HasValue;
        var gespeichertePflanzeId = pflanze.Id;

        try
        {
            db.SaveChanges();
            gespeichertePflanzeId = pflanze.Id;
        }
        catch (DbUpdateException ex)
        {
            StatusSetzen($"Speichern fehlgeschlagen: {ex.GetBaseException().Message}");
            return;
        }

        PflanzeTabZuruecksetzen();
        LadeDaten();
        var erneutAuswaehlen = _pflanzen.FirstOrDefault(x => x.Id == gespeichertePflanzeId);
        if (erneutAuswaehlen is not null)
        {
            PflanzenDataGrid.SelectedItem = erneutAuswaehlen;
        }

        var statusText = warBearbeiten ? "Pflanze aktualisiert." : "Pflanze gespeichert.";
        MainTabs.SelectedIndex = 0;
        StatusSetzen(statusText);
    }

    private void PflanzeTabZuruecksetzen()
    {
        _pflanzeIdZumBearbeiten = null;
        TabNameBox.Text = string.Empty;
        TabArtBox.Text = string.Empty;
        TabStandortBox.Text = string.Empty;
        TabNotizenBox.Text = string.Empty;
        TabKaufdatumPicker.SelectedDate = DateTimeOffset.Now;
    }

    private void LadeRegelInFormular()
    {
        if (RegelnDataGrid.SelectedItem is not RegelViewItem regel)
        {
            return;
        }

        _regelIdZumBearbeiten = regel.Id;
        TabRegelTypComboBox.SelectedItem = regel.Typ;
        TabRegelIntervallBox.Text = regel.Intervall.Replace(" Tage", string.Empty);
        if (DateTime.TryParse(regel.Startdatum, out var parsed))
        {
            TabRegelStartdatumPicker.SelectedDate = new DateTimeOffset(parsed);
        }
    }

    private void RegelAusTabSpeichern()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst in Übersicht eine Pflanze auswählen.");
            return;
        }

        var typ = TabRegelTypComboBox.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        var intervallText = (TabRegelIntervallBox.Text ?? string.Empty).Trim();
        var startdatum = TabRegelStartdatumPicker.SelectedDate;
        if (string.IsNullOrWhiteSpace(typ) || !int.TryParse(intervallText, out var intervall) || intervall <= 0 || startdatum is null)
        {
            StatusSetzen("Regel ist unvollständig oder ungültig.");
            return;
        }

        using var db = new AppDbContext();
        PflegeRegel regel;
        if (_regelIdZumBearbeiten is null)
        {
            regel = new PflegeRegel { PflanzeId = _ausgewaehltePflanze.Id };
            db.PflegeRegeln.Add(regel);
        }
        else
        {
            regel = db.PflegeRegeln.FirstOrDefault(x => x.Id == _regelIdZumBearbeiten.Value && x.PflanzeId == _ausgewaehltePflanze.Id)
                ?? new PflegeRegel { PflanzeId = _ausgewaehltePflanze.Id };
            if (regel.Id == 0)
            {
                db.PflegeRegeln.Add(regel);
            }
        }

        regel.Typ = typ;
        regel.IntervallTage = intervall;
        regel.Startdatum = startdatum.Value.DateTime.Date;
        db.SaveChanges();

        LadeRegelnFuerAuswahl();
        RegelTabZuruecksetzen();
        StatusSetzen("Regel gespeichert.");
    }

    private void RegelTabZuruecksetzen()
    {
        _regelIdZumBearbeiten = null;
        TabRegelTypComboBox.SelectedIndex = 0;
        TabRegelIntervallBox.Text = string.Empty;
        TabRegelStartdatumPicker.SelectedDate = DateTimeOffset.Now;
        RegelnDataGrid.SelectedItem = null;
    }

    private void LadeEintragInFormular()
    {
        if (TabEintraegeDataGrid.SelectedItem is not PflegeEintragViewItem eintrag)
        {
            return;
        }

        TabEintragTypComboBox.SelectedItem = eintrag.Typ;
        if (DateTime.TryParse(eintrag.Datum, out var parsed))
        {
            TabEintragDatumPicker.SelectedDate = new DateTimeOffset(parsed);
        }

        TabEintragMengeBox.Text = eintrag.Menge;
        TabEintragNotizBox.Text = eintrag.Notiz;
    }

    private void EintragAusTabHinzufuegen()
    {
        if (_ausgewaehltePflanze is null)
        {
            StatusSetzen("Bitte zuerst in Übersicht eine Pflanze auswählen.");
            return;
        }

        var typ = TabEintragTypComboBox.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        var datum = TabEintragDatumPicker.SelectedDate;
        if (string.IsNullOrWhiteSpace(typ) || datum is null)
        {
            StatusSetzen("Eintrag ist unvollständig.");
            return;
        }

        using var db = new AppDbContext();
        db.PflegeEintraege.Add(new PflegeEintrag
        {
            PflanzeId = _ausgewaehltePflanze.Id,
            Typ = typ,
            Datum = datum.Value.DateTime.Date,
            Menge = (TabEintragMengeBox.Text ?? string.Empty).Trim(),
            Notiz = (TabEintragNotizBox.Text ?? string.Empty).Trim()
        });
        db.SaveChanges();

        LadeEintraegeFuerAuswahl();
        EintragTabZuruecksetzen();
        StatusSetzen("Eintrag hinzugefügt.");
    }

    private void EintragAusTabAendern()
    {
        if (_ausgewaehltePflanze is null || TabEintraegeDataGrid.SelectedItem is not PflegeEintragViewItem eintrag)
        {
            StatusSetzen("Bitte zuerst einen Eintrag auswählen.");
            return;
        }

        var typ = TabEintragTypComboBox.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        var datum = TabEintragDatumPicker.SelectedDate;
        if (string.IsNullOrWhiteSpace(typ) || datum is null)
        {
            StatusSetzen("Eintrag ist unvollständig.");
            return;
        }

        using var db = new AppDbContext();
        var entity = db.PflegeEintraege.FirstOrDefault(x => x.Id == eintrag.Id && x.PflanzeId == _ausgewaehltePflanze.Id);
        if (entity is null)
        {
            StatusSetzen("Eintrag wurde nicht gefunden.");
            return;
        }

        entity.Typ = typ;
        entity.Datum = datum.Value.DateTime.Date;
        entity.Menge = (TabEintragMengeBox.Text ?? string.Empty).Trim();
        entity.Notiz = (TabEintragNotizBox.Text ?? string.Empty).Trim();
        db.SaveChanges();

        LadeEintraegeFuerAuswahl();
        StatusSetzen("Eintrag aktualisiert.");
    }

    private async System.Threading.Tasks.Task EintragAusTabLoeschen()
    {
        if (_ausgewaehltePflanze is null || TabEintraegeDataGrid.SelectedItem is not PflegeEintragViewItem eintrag)
        {
            StatusSetzen("Bitte zuerst einen Eintrag auswählen.");
            return;
        }

        var bestaetigt = await BestaetigungsDialog(
            "Löschen bestätigen",
            $"Möchtest du den Eintrag vom {eintrag.Datum} ({eintrag.Typ}) wirklich löschen?",
            "Ja, löschen");
        if (!bestaetigt)
        {
            StatusSetzen("Löschen abgebrochen.");
            return;
        }

        using var db = new AppDbContext();
        var entity = db.PflegeEintraege.FirstOrDefault(x => x.Id == eintrag.Id && x.PflanzeId == _ausgewaehltePflanze.Id);
        if (entity is null)
        {
            StatusSetzen("Eintrag wurde nicht gefunden.");
            return;
        }

        db.PflegeEintraege.Remove(entity);
        db.SaveChanges();
        LadeEintraegeFuerAuswahl();
        EintragTabZuruecksetzen();
        StatusSetzen("Eintrag gelöscht.");
    }

    private void EintragTabZuruecksetzen()
    {
        TabEintragTypComboBox.SelectedIndex = 0;
        TabEintragDatumPicker.SelectedDate = DateTimeOffset.Now;
        TabEintragMengeBox.Text = string.Empty;
        TabEintragNotizBox.Text = string.Empty;
        TabEintraegeDataGrid.SelectedItem = null;
    }

    private void StatusSetzen(string text)
    {
        StatusText.Text = text;
    }

    private static string? ValidierePflanze(string name, string art, string standort, DateTimeOffset? kaufdatum)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(art) ||
            string.IsNullOrWhiteSpace(standort) ||
            kaufdatum is null)
        {
            return "Bitte alle Pflichtfelder für Pflanze ausfüllen.";
        }

        return null;
    }

    private static bool IstNameStandortDuplikat(IEnumerable<Pflanze> pflanzen, string name, string standort, int? ausnahmeId = null)
    {
        return pflanzen.Any(p =>
            p.Name == name &&
            p.Standort == standort &&
            (!ausnahmeId.HasValue || p.Id != ausnahmeId.Value));
    }

    private async System.Threading.Tasks.Task<bool> BestaetigungsDialog(string titel, string text, string bestaetigenText)
    {
        var dialog = new Window
        {
            Width = 460,
            Height = 180,
            Title = titel,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var message = new TextBlock
        {
            Text = text,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 16)
        };

        var jaButton = new Button
        {
            Content = bestaetigenText,
            Width = 120,
            Height = 36
        };
        var neinButton = new Button
        {
            Content = "Abbrechen",
            Width = 120,
            Height = 36
        };

        jaButton.Click += (_, _) => dialog.Close(true);
        neinButton.Click += (_, _) => dialog.Close(false);

        dialog.Content = new StackPanel
        {
            Margin = new Thickness(16),
            Spacing = 8,
            Children =
            {
                message,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 10,
                    Children = { neinButton, jaButton }
                }
            }
        };

        var result = await dialog.ShowDialog<bool>(this);
        return result;
    }
}
