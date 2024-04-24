﻿using System.ComponentModel.DataAnnotations;

namespace hotel_backend.API.Models
{
    public class Customer
    {
        [Required]
        public Guid Id { get; set; }
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Room>? Rooms {  get; set; }  
    }
}
