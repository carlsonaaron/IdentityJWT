using System.ComponentModel.DataAnnotations;

namespace IdentityJwtAPI.ViewModels
{
    public class AccountViewModels
    {
        public interface IAccountViewModel
        {
            public string Email { get; set; }
        }

        public class LoginViewModel: IAccountViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }


        public class RegisterViewModel: IAccountViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }


            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
            public string ConfirmPassword { get; set; }
        }

        public class AccountViewModel: IAccountViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
