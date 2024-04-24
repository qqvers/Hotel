using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models.Domain
{
    public class Room
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }

        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required]
        public Guid OwnerId { get; set; }
        public Owner Owner { get; set; }
    }
}
