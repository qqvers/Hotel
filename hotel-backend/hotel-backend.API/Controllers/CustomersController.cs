using hotel_backend.API.Data;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly HotelDbContext _hotelDbContext;
        public CustomersController(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }

        private const string SecretKey = "SecretKeySecretKeySecretKeySecretKey";
        private readonly SymmetricSecurityKey loginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


        /// <summary>
        /// Signs up a new customer.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object containing the new customer's details.</param>
        /// <returns>A success message if the signup was successful.</returns>
        /// <response code="200">User added successfully.</response>
        /// <response code="400">If the customer's details are invalid or the email is already in use.</response>
        [HttpPost("signup/customer")]
        [AllowAnonymous]
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


            if (await _hotelDbContext.Customers.AnyAsync(o => o.Email == customerDto.Email && o.Id != customer.Id))
            {
                return BadRequest("Email already in use by another customer");
            }

            var passwordHasher = new PasswordHasher<Customer>();
            customer.Password = passwordHasher.HashPassword(customer, customerDto.Password);

            _hotelDbContext.Customers.Add(customer);
            await _hotelDbContext.SaveChangesAsync();


            return Ok("User added successfully");
        }

        /// <summary>
        /// Logs in a customer and returns a JWT token.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object for login credentials.</param>
        /// <returns>A JWT token if the login was successful.</returns>
        /// <response code="200">Login successful, token generated.</response>
        /// <response code="401">If the login credentials are invalid.</response>
        /// <response code="404">If the email does not exist in the database.</response>
        [HttpPost("login/customer")]
        [AllowAnonymous]
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
                new Claim("UserType", "Customer"),
                new Claim("Id", (customerInDatabase.Id).ToString())
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
                name = customerInDatabase.Name,
                id = customerInDatabase.Id
            });
        }

        /// <summary>
        /// Updates the profile of an existing customer.
        /// </summary>
        /// <param name="id">The unique identifier of the customer.</param>
        /// <param name="customerDto">The customer data transfer object containing the new details.</param>
        /// <returns>A success message if the update was successful.</returns>
        /// <response code="200">Customer profile updated successfully.</response>
        /// <response code="400">If the customer's new details are invalid or the email is already in use.</response>
        /// <response code="404">If the customer with the given ID was not found.</response>
        [HttpPut("update/customer/{id}")]
        [Authorize(Policy = "IsCustomer")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _hotelDbContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("customer not found");
            }

            if (await _hotelDbContext.Customers.AnyAsync(o => o.Email == customerDto.Email && o.Id != id))
            {
                return BadRequest("Email already in use by another customer");
            }

            var passwordHasher = new PasswordHasher<Customer>();

            customer.Name = customerDto.Name;
            customer.Email = customerDto.Email;
            customer.Password = passwordHasher.HashPassword(customer, customerDto.Password);


            _hotelDbContext.Customers.Update(customer);
            await _hotelDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "customer profile updated successfully",
                customerId = customer.Id,
                customerName = customer.Name,
                customerEmail = customer.Email
            });
        }

    }
}
