using Microsoft.AspNetCore.Identity;
namespace Abcmoney_Transfer.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = "Rajen";
        public string MiddleName { get; set; } = "Rajen";
        public string LastName { get; set; } = "Rajen";
        public string Address { get; set; } = "Dhangadi";
        public string Country { get; set; } = "Nepal";
    }
}
