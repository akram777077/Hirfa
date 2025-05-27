using System;
using Hirfa.Web.Models;

namespace Hirfa.Web.ViewModels;

public class DemandeClientViewModel
{
    public int Id { get; set; }

    public DateTime DateDemande { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public DemandeclientStatus Etat { get; set; }

    public string? Description { get; set; }

    public string Categorie { get; set; } = null!;

    public string? ClientAddress { get; set; } // Added property for client address

    public int MatchingPrestatairesCount { get; set; } // Added property for matching prestataires count

    public string ClientName { get; set; } = null!;

    public string ClientGender { get; set; } = null!;

    public int? DevisId { get; set; } // Added property for Devis ID

    public DevisViewModel? Devis { get; set; }

    public DateTime Datedebut { get; set; }
    public DateTime? Datefin { get; set; }
    public DateTime Datedemande { get; set; }

    public bool HasSentDevis => DevisId.HasValue;
}
