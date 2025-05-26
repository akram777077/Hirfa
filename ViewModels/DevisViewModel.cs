using System;
using System.Collections.Generic;
using Hirfa.Web.Models;

namespace Hirfa.Web.ViewModels
{
    public class DevisViewModel
    {
        public int Iddevis { get; set; }
        public string Etat { get; set; } = null!;
        public decimal Montantglobal { get; set; }
        public DateTime? Datelimite { get; set; }
        public string? Avisclient { get; set; }
        public int Idprestataire { get; set; }
        public int Iddemandeclient { get; set; }
        public List<QuantiteMatiereDevisViewModel> QuantiteMatieres { get; set; } = new();

        // New property for demand details
        public DemandeClientViewModel DemandDetails { get; set; } = new();

        // Restored property for devis-specific description
        public string? Description { get; set; }
    }

    public class QuantiteMatiereDevisViewModel
    {
        public int Idmatierepremiere { get; set; }
        public string MatierePremiereName { get; set; } = null!;
        public decimal Quantite { get; set; }
    }
}
