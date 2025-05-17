using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Devi
{
    public int Iddevis { get; set; }

    public string Etat { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Montantglobal { get; set; }

    public DateOnly? Datelimite { get; set; }

    public string? Avisclient { get; set; }

    public int Idprestataire { get; set; }

    public int Iddemandeclient { get; set; }

    public virtual Demandeclient IddemandeclientNavigation { get; set; } = null!;

    public virtual Prestataire IdprestataireNavigation { get; set; } = null!;

    public virtual ICollection<Quantitematieredevi> Quantitematieredevis { get; set; } = new List<Quantitematieredevi>();
}
