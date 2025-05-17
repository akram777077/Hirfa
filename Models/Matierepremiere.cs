using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Matierepremiere
{
    public int Idmatierepremiere { get; set; }

    public string Nommat { get; set; } = null!;

    public decimal Prixmat { get; set; }

    public virtual ICollection<Quantitematieredevi> Quantitematieredevis { get; set; } = new List<Quantitematieredevi>();
}
