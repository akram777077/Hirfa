using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Facture
{
    public int Idfacture { get; set; }

    public DateOnly Dateemission { get; set; }

    public DateOnly? Dateecheance { get; set; }

    public string? Description { get; set; }

    public string? Modepaiement { get; set; }

    public int Iddemandeclient { get; set; }

    public virtual Demandeclient IddemandeclientNavigation { get; set; } = null!;

    public virtual ICollection<Quantitematierefacture> Quantitematierefactures { get; set; } = new List<Quantitematierefacture>();
}
