using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Evaluation
{
    public int Idevaluation { get; set; }

    public string? Commentaire { get; set; }

    public int Note { get; set; }

    public string? Image { get; set; }

    public int Idclient { get; set; }

    public int Idprestataire { get; set; }

    public virtual Client IdclientNavigation { get; set; } = null!;

    public virtual Prestataire IdprestataireNavigation { get; set; } = null!;
}
