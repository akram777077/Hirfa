using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Prestataire
{
    public int Idprestataire { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Numerotelephone { get; set; }

    public DateOnly? Datenaissance { get; set; }

    public string? Adresse { get; set; }

    public string? Sexe { get; set; }

    public bool Estdisponible { get; set; }

    public string Nin { get; set; } = null!;

    public string Typeservice { get; set; } = null!;

    public string? Cv { get; set; }

    public string? Casierjudiciaire { get; set; }

    public int Idcompte { get; set; }

    public int Iddemandeprestataire { get; set; }

    public virtual ICollection<Demandeclient> Demandeclients { get; set; } = new List<Demandeclient>();

    public virtual ICollection<Devi> Devis { get; set; } = new List<Devi>();

    public virtual ICollection<Diplome> Diplomes { get; set; } = new List<Diplome>();

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public virtual Compte IdcompteNavigation { get; set; } = null!;

    public virtual Demandeprestataire IddemandeprestataireNavigation { get; set; } = null!;

    public virtual ICollection<Plainteclient> Plainteclients { get; set; } = new List<Plainteclient>();

    public virtual ICollection<Plainteprestataire> Plainteprestataires { get; set; } = new List<Plainteprestataire>();
}
