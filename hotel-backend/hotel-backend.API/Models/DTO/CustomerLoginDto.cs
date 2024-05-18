using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models.DTO
{
    public class CustomerLoginDto
    {

        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
