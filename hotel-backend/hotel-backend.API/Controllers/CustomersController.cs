using hotel_backend.API.Data;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace hotel_backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly HotelDbContext _hotelDbContext;
        public CustomersController(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }

        private const string SecretKey = "SecretKeySecretKeySecretKeySecretKey";
        private readonly SymmetricSecurityKey loginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


        [HttpPost("signup/customer")]
        public async Task<IActionResult> SignUpCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = new Customer()
            {
                Name = customerDto.Name,
                Email = customerDto.Email,
                Password = customerDto.Password,
            };
            var passwordHasher = new PasswordHasher<Customer>();
            customer.Password = passwordHasher.HashPassword(customer, customerDto.Password);

            _hotelDbContext.Customers.Add(customer);
            await _hotelDbContext.SaveChangesAsync();


            return Ok("User added successfully");
        }

        [HttpPost("login/customer")]
        public async Task<IActionResult> LoginCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerInDatabase = await _hotelDbContext.Customers.SingleOrDefaultAsync(o => o.Email == customerDto.Email);

            if (customerInDatabase == null)
            {
                return NotFound("Provided email does not exist in database");
            }

            var passwordHasher = new PasswordHasher<Customer>();
            var result = passwordHasher.VerifyHashedPassword(customerInDatabase, customerInDatabase.Password, customerDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customerInDatabase.Email),
                new Claim(ClaimTypes.Name, customerInDatabase.Name),
                new Claim("UserType", "Customer")
            };

            var creds = new SigningCredentials(loginKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                expiration = token.ValidTo,
                name = customerInDatabase.Name
            });
        }

    }
}
