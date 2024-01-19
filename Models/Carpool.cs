using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarpooliDotTN.Models
{
    public class Carpool
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime CreationTime { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }

        [Range(0, 20, ErrorMessage = "Price must be between 0 and 20.")]
        public double Price { get; set; }

        [Range(0, 7, ErrorMessage = "Number of places must be between 0 and 7.")]
        public int NumberOfPlaces { get; set; }
        [Display(Name ="Availability")]
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        [InverseProperty("Carpool")]
        public virtual ICollection<Demand> Demands { get; set; }
        public virtual User Owner { get; set; }
    }
}
