using System;
using System.Collections.Generic;

namespace TerminRessourcenPlaner.ViewModels
{
    public class HomeDashboardViewModel
    {
        // Kopfbereich
        public DateTime Datum { get; set; } = DateTime.Today;

        public DateTime TagesStart { get; set; }
        public DateTime TagesEnde { get; set; }

        // KPI-Karten
        public int TermineHeuteAnzahl { get; set; }
        public int RessourcenGesamtAnzahl { get; set; }
        public int RessourcenBelegtJetztAnzahl { get; set; }
        public int RessourcenFreiJetztAnzahl { get; set; }

        // Nächster Termin (optional)
        public TerminKurzInfo? NaechsterTermin { get; set; }

        // Heutige Termine (Liste)
        public List<TerminKurzInfo> HeutigeTermine { get; set; } = new();

        // Ressourcen-Status (Mitarbeiter / Stühle)
        public List<RessourceStatusInfo> RessourcenStatusHeute { get; set; } = new();
    }

    public class TerminKurzInfo
    {
        public int TerminId { get; set; }
        public string Titel { get; set; } = string.Empty;

        public DateTime Start { get; set; }
        public DateTime Ende { get; set; }

        public int ZugewieseneRessourcenAnzahl { get; set; }

        // Optional: Namen der Ressourcen (z. B. Mitarbeiter)
        public List<string> RessourcenNamen { get; set; } = new();
    }

    public class RessourceStatusInfo
    {
        public int RessourceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Typ { get; set; } = string.Empty;

        public List<TerminBlockInfo> BuchungenHeute { get; set; } = new();


        // Status für "heute" / "jetzt"
        public bool IstGeradeBelegt { get; set; }

        // Wenn belegt: Zeitraum + Termininfo
        public DateTime? BelegtVon { get; set; }
        public DateTime? BelegtBis { get; set; }
        public string? TerminTitel { get; set; }
        public int? TerminId { get; set; }
    }
    public class TerminBlockInfo
    {
        public int TerminId { get; set; }
        public string Titel { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime Ende { get; set; }
    }
}