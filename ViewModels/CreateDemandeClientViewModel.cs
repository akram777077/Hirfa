using System.ComponentModel.DataAnnotations;

namespace Hirfa.Web.ViewModels
{
    public class CreateDemandeClientViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Categorie { get; set; } = null!;

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DateValidation), nameof(DateValidation.ValidateNotPastDate))]
        public DateTime Datedebut { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Datefin { get; set; }
    }
}
