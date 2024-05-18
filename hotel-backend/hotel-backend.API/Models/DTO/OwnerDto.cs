using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models.DTO
{
    public class OwnerDto
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
