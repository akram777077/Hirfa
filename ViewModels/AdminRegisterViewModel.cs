using System.ComponentModel.DataAnnotations;

namespace Hirfa.Web.ViewModels
{
    public class AdminRegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
