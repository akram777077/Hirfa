using System;
using Hirfa.Web.Models;

namespace Hirfa.Web.ViewModels;

public class DemandeClientViewModel
{
    public int Id { get; set; }

    public DateTime DateDemande { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public DemandeclientStatus Etat { get; set; }

    public string? Description { get; set; }

    public string Categorie { get; set; } = null!;
}
