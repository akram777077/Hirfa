namespace Hirfa.Web.ViewModels;

public class PrestataireViewModel
{
    public int Idprestataire { get; set; }

    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public string? Numerotelephone { get; set; }

    public string? Adresse { get; set; }

    public string Typeservice { get; set; } = null!;

    public bool Estdisponible { get; set; }

    public string? Sexe { get; set; }

    public int Stars { get; set; }

    public int TotalDemands { get; set; }

    public bool HasRejectedDevis { get; set; }
}
