using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarpooliDotTN.Models
{
    public class Demand
    {
        public Guid Id { get; set; }
        [ForeignKey("Passenger")]
        public string PassengerId { get; set; }
        [ForeignKey("Carpool")]
        public Guid CarpoolId { get; set; }
        public enum Response { pending,accepted,refused }
        public Response status { get; set; }
        public DateTime SubmissionTime { get; set; }
        public virtual Carpool Carpool { get; set; }
        public virtual User Passenger { get; set; }
    }
}
