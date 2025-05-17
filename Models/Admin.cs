using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Admin
{
    public int Idadmin { get; set; }

    public int Idcompte { get; set; }

    public virtual Compte IdcompteNavigation { get; set; } = null!;
}
