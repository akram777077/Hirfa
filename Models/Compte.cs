using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Compte
{
    public int Idcompte { get; set; }

    public string Email { get; set; } = null!;

    public string Motdepasse { get; set; } = null!;

    public virtual Admin? Admin { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Prestataire? Prestataire { get; set; }

    public virtual Serviceclient? Serviceclient { get; set; }
}
