using System;
using System.Collections.Generic;

namespace Hirfa.Web.Models;

public partial class Demandeclient
{
    public int Iddemandeclient { get; set; }

    public DateTime Datedemande { get; set; }

    public DateTime Datedebut { get; set; }

    public DateTime? Datefin { get; set; }

    public DemandeclientStatus Etat { get; set; }

    public string? Description { get; set; }

    public string Categorie { get; set; } = null!;

    public int Idclient { get; set; }

    public int? Idprestataire { get; set; }

    public virtual ICollection<Devi> Devis { get; set; } = new List<Devi>();

    public virtual Facture? Facture { get; set; }

    public virtual Client IdclientNavigation { get; set; } = null!;

    public virtual Prestataire? IdprestataireNavigation { get; set; }
}
