using hotel_backend.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models.DTO
{
    public class OwnerDto
    {
        public string? Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
