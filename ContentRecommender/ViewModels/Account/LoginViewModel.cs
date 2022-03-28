using System.ComponentModel.DataAnnotations;

namespace ContentRecommender.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [StringLength(320, MinimumLength = 3, ErrorMessage = "Email address must be between {2} and {1} characters")]
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Password must be between {2} and {1} characters")]
        public string Password { get; set; }
    }
}
