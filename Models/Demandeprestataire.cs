using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Demandeprestataire
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

    // For mapping to DemandeclientStatus in Demandeclient
    public string Etat { get; set; } = null!;

    public string? Casierjudiciaire { get; set; }

    public virtual ICollection<Diplomedemande> Diplomedemandes { get; set; } = new List<Diplomedemande>();

    public virtual ICollection<Prestataire> Prestataires { get; set; } = new List<Prestataire>();
}
