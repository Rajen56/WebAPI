using System.ComponentModel.DataAnnotations;

namespace Abcmoney_Transfer.Viewmodel
{
    public class LoginInputVM
    {
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(100, ErrorMessage = "UserName cannot exceed 100 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long and cannot exceed 100 characters.")]
        public string Password { get; set; }
    }
    public class RegisterInputVM
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email must contain the '@' symbol and a valid domain.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$", ErrorMessage = "Password must contain at least one letter, one number, and no special characters.")]
        public string Password { get; set; }
    }

}
