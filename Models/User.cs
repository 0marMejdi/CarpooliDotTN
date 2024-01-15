using Microsoft.AspNetCore.Identity;

namespace CarpooliDotTN.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Ville { get; set; }
        public string? Genre { get; set; }
        public string? NumTel { get; set; }

    }
}
