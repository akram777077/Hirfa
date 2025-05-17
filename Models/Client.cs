using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Client
{
    public int Idclient { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Numerotelephone { get; set; }

    public DateOnly? Datenaissance { get; set; }

    public string? Adresse { get; set; }

    public string? Sexe { get; set; }

    public int Idcompte { get; set; }

    public virtual ICollection<Demandeclient> Demandeclients { get; set; } = new List<Demandeclient>();

    public virtual ICollection<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

    public virtual Compte IdcompteNavigation { get; set; } = null!;

    public virtual ICollection<Plainteclient> Plainteclients { get; set; } = new List<Plainteclient>();

    public virtual ICollection<Plainteprestataire> Plainteprestataires { get; set; } = new List<Plainteprestataire>();
}
