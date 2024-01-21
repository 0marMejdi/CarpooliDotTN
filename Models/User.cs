using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CarpooliDotTN.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace CarpooliDotTN.Models
{
   
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? Gender { get; set; }
        [Phone]
        [Required]
        public string PhoneNumber {  get; set; }
        public ICollection<Carpool> Carpools { get; set; }
        public ICollection<Demand> Demands { get; set; }
    
    }
}
