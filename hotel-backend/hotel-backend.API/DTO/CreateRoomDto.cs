using hotel_backend.API.Models;
using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.DTO
{
    public class CreateRoomDto
    {
        public string Name { get; set; }
        public bool Available { get; set; }
        public Guid OwnerId { get; set; }

    }
}
