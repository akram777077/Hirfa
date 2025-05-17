using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Serviceclient
{
    public int Idserviceclient { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Numerotelephone { get; set; }

    public DateOnly? Datenaissance { get; set; }

    public string? Adresse { get; set; }

    public string? Sexe { get; set; }

    public int Idcompte { get; set; }

    public virtual Compte IdcompteNavigation { get; set; } = null!;
}
