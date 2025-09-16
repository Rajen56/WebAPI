using Microsoft.AspNetCore.Identity;

namespace Abcmoney_Transfer.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = "Admin";
        public string MiddleName { get; set; } = "Admin";
        public string LastName { get; set; } = "Admin";
        public string Address { get; set; } = "Kathmandu";
        public string Country { get; set; } = "Nepal";
    }
}
