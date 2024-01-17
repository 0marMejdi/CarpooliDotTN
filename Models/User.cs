using CarpooliDotTN.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarpooliDotTN.Models
{
   
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? Gender { get; set; }

        public ICollection<Carpool> Carpools { get; set; }
        public ICollection<Demand> Demands { get; set; }

    }
}
