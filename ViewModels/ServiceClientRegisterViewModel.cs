using System;
using System.ComponentModel.DataAnnotations;

namespace Hirfa.Web.ViewModels
{
    public class ServiceClientRegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

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
        public string Sexe { get; set; } = null!;
    }
}
