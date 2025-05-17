using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Plainteclient
{
    public int Idplainteclient { get; set; }

    public string Sujet { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly Date { get; set; }

    public string Contenu { get; set; } = null!;

    public int Idprestataire { get; set; }

    public int Idclient { get; set; }

    public virtual Client IdclientNavigation { get; set; } = null!;

    public virtual Prestataire IdprestataireNavigation { get; set; } = null!;
}
