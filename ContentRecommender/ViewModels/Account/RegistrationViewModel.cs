using System.ComponentModel.DataAnnotations;

namespace ContentRecommender.ViewModels.Account
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Username must be between {2} and {1} characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [StringLength(320, MinimumLength = 3, ErrorMessage = "Email address must be between {2} and {1} characters")]
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Password must be between {2} and {1} characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "The passwords do not match")]
        public string ConfirmedPassword { get; set; }
    }
}
