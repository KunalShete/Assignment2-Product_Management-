using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ProductManagement.Models.ViewModel
{
    public class AddAdminViewModel
    {
        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
