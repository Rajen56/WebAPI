using Microsoft.AspNetCore.Identity;
namespace Abcmoney_Transfer.Models
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = "Rajen";
        public string MiddleName { get; set; } = "Prasad";
        public string LastName { get; set; } = "Bhatta";
        public string Address { get; set; } = "Kathmandu";
        public string Country { get; set; } = "Nepal";
    }
}
