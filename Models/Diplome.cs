using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Diplome
{
    public int Iddiplome { get; set; }

    public string Institution { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? Anneeobtention { get; set; }

    public string? Fiche { get; set; }

    public int Idprestataire { get; set; }

    public virtual Prestataire IdprestataireNavigation { get; set; } = null!;
}
