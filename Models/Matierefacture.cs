using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Matierefacture
{
    public int Idmatierefacture { get; set; }

    public string Nommat { get; set; } = null!;

    public decimal Prixmat { get; set; }

    public virtual ICollection<Quantitematierefacture> Quantitematierefactures { get; set; } = new List<Quantitematierefacture>();
}
