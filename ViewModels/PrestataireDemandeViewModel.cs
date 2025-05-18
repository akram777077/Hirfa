using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Hirfa.Web.ViewModels
{
    public class PrestataireDemandeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Prenom { get; set; } = null!;

        [Required]
        public string Nom { get; set; } = null!;

        [Required]
        public string Numerotelephone { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Datenaissance { get; set; }

        [Required]
        public string Adresse { get; set; } = null!;

        [Required]
        public string Nin { get; set; } = null!;

        [Required]
        public string Typeservice { get; set; } = null!;

        public IFormFile? CvFile { get; set; }
        public IFormFile? CasierjudiciaireFile { get; set; }
        public List<DiplomeDemandeInputModel>? Diplomes { get; set; }
    }

    public class DiplomeDemandeInputModel
    {
        [Required]
        public string Institution { get; set; } = null!;
        [Required]
        public string Type { get; set; } = null!;
        [Required]
        [Display(Name = "Year Obtained")]
        public int AnneeObtention { get; set; }
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
