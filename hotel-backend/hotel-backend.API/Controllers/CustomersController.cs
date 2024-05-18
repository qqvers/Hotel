using AutoMapper;
using hotel_backend.API.Data;
using hotel_backend.API.Data.Interfaces;
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
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomersController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        private const string SecretKey = "SecretKeySecretKeySecretKeySecretKey";
        private readonly SymmetricSecurityKey loginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


        /// <summary>
        /// Signs up a new customer.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object containing the new customer's details.</param>
        /// <returns>A success message if the signup was successful.</returns>
        /// <response code="201">User added successfully.</response>
        /// <response code="400">If the customer's details are invalid or the email is already in use.</response>
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var customer = await _customerRepository.SignUpCustomer(customerDto);
                return Created("", new { message = "User added successfully", userId = customer.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        /// <summary>
        /// Logs in a customer and returns a JWT token.
        /// </summary>
        /// <param name="customerDto">The customer data transfer object for login credentials.</param>
        /// <returns>A JWT token if the login was successful.</returns>
        /// <response code="200">Login successful, token generated.</response>
        /// <response code="401">If the login credentials are invalid.</response>
        /// <response code="404">If the email does not exist in the database.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCustomer([FromBody] CustomerLoginDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerRepository.LoginCustomer(customerDto);
            if (customer == null)
            {
                return NotFound("Provided email does not exist in database");
            }

            var passwordHasher = new PasswordHasher<Customer>();
            var result = passwordHasher.VerifyHashedPassword(customer, customer.Password, customerDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Email),
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim("UserType", "Customer"),
                new Claim("Id", customer.Id.ToString())
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
                name = customer.Name,
                id = customer.Id
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
        [HttpPut("{id}")]
        [Authorize(Policy = "IsCustomer")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _customerRepository.FindCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            if (await _customerRepository.CheckEmailExistenceAsync(id, customerDto.Email))
            {
                return BadRequest("Email already in use by another customer.");
            }

            var passwordHasher = new PasswordHasher<Customer>();
            customerDto.Password = passwordHasher.HashPassword(customer, customerDto.Password);

            customer = _mapper.Map<Customer>(customerDto);

            await _customerRepository.UpdateCustomerAsync(customer);

            return Ok(new
            {
                message = "Customer profile updated successfully",
                customerId = customer.Id,
                customerName = customer.Name,
                customerEmail = customer.Email
            });
        }

    }
}
