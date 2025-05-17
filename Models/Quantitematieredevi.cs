using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Quantitematieredevi
{
    public int Iddevis { get; set; }

    public int Idmatierepremiere { get; set; }

    public decimal Quantite { get; set; }

    public virtual Devi IddevisNavigation { get; set; } = null!;

    public virtual Matierepremiere IdmatierepremiereNavigation { get; set; } = null!;
}
