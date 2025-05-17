using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Diplomedemande
{
    public int Iddiplomedemande { get; set; }

    public string Institution { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? Anneeobtention { get; set; }

    public string? Fiche { get; set; }

    public int Iddemandeprestataire { get; set; }

    public virtual Demandeprestataire IddemandeprestataireNavigation { get; set; } = null!;
}
