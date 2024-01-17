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
        public double Price { get; set; }
        public int NumberOfPlaces { get; set; }
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        [InverseProperty("Carpool")]
        public virtual ICollection<Demand> Demands { get; set; }
        public virtual User Owner { get; set; }
    }
}
