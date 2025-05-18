using System;
using System.Collections.Generic;

namespace Hirfa.Web.ViewModels
{
    public class PrestataireDemandeListItemViewModel
    {
        public int Iddemandeprestataire { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Typeservice { get; set; } = null!;
        public string Etat { get; set; } = null!;
    }

    public class PrestataireDemandeDetailViewModel
    {
        public int Iddemandeprestataire { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public DateOnly? Datenaissance { get; set; }
        public string? Numtel { get; set; }
        public string Nin { get; set; } = null!;
        public string Typeservice { get; set; } = null!;
        public string? Adresse { get; set; }
        public string Email { get; set; } = null!;
        public string? Cv { get; set; }
        public string Etat { get; set; } = null!;
        public string? Casierjudiciaire { get; set; }
        public string? Reason { get; set; } // Add this property
        public List<DiplomeDemandeDetailViewModel> Diplomes { get; set; } = new();
    }

    public class DiplomeDemandeDetailViewModel
    {
        public string Institution { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int? Anneeobtention { get; set; }
        public string? Fiche { get; set; }
    }
}
