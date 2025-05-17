using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Quantitematierefacture
{
    public int Idfacture { get; set; }

    public int Idmatierefacture { get; set; }

    public decimal Quantite { get; set; }

    public virtual Facture IdfactureNavigation { get; set; } = null!;

    public virtual Matierefacture IdmatierefactureNavigation { get; set; } = null!;
}
