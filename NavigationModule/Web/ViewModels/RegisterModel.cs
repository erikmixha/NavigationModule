using System.ComponentModel.DataAnnotations;
namespace NavigationModule.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 256 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        public string Role { get; set; }


    }

}
