using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models.DTO
{
    public class CustomerDto
    {
        public string? Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
