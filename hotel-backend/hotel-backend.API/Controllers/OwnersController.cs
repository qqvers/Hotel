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
    public class OwnersController : ControllerBase
    {
        private readonly HotelDbContext _hotelDbContext;
        public OwnersController(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }

        private const string SecretKey = "SecretKeySecretKeySecretKeySecretKey";
        private readonly SymmetricSecurityKey loginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));


        [HttpPost("signup/owner")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] OwnerDto ownerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = new Owner()
            {
                Name = ownerDto.Name,
                Email = ownerDto.Email,
                Password = ownerDto.Password,
                Rooms = new List<Room>(),
            };

            if (await _hotelDbContext.Owners.AnyAsync(o => o.Email == ownerDto.Email && o.Id != owner.Id))
            {
                return BadRequest("Email already in use by another owner");
            }

            var passwordHasher = new PasswordHasher<Owner>();
            owner.Password = passwordHasher.HashPassword(owner, ownerDto.Password);

            _hotelDbContext.Owners.Add(owner);
            await _hotelDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login/owner")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] OwnerDto ownerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerInDatabase = await _hotelDbContext.Owners
                                     .SingleOrDefaultAsync(o => o.Email == ownerDto.Email); 
            if (ownerInDatabase == null)
            {
                return NotFound("Provided email does not exist in database");
            }

            var passwordHasher = new PasswordHasher<Owner>();
            var result = passwordHasher.VerifyHashedPassword(ownerInDatabase, ownerInDatabase.Password, ownerDto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, ownerInDatabase.Email),
                new Claim(ClaimTypes.Name, ownerInDatabase.Name),
                new Claim("UserType", "Owner"),
                new Claim("Id", (ownerInDatabase.Id).ToString())
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
                name = ownerInDatabase.Name,
                id = ownerInDatabase.Id
            });
        }

        [HttpPut("update/owner/{id}")]
        [Authorize(Policy = "IsOwner")]
        public async Task<IActionResult> UpdateOwner(Guid id, [FromBody] OwnerDto updateOwnerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var owner = await _hotelDbContext.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound("Owner not found");
            }

            if (await _hotelDbContext.Owners.AnyAsync(o => o.Email == updateOwnerDto.Email && o.Id != id))
            {
                return BadRequest("Email already in use by another owner");
            }

            var passwordHasher = new PasswordHasher<Owner>();

            owner.Name = updateOwnerDto.Name;
            owner.Email = updateOwnerDto.Email;
            owner.Password = passwordHasher.HashPassword(owner, updateOwnerDto.Password); 


            _hotelDbContext.Owners.Update(owner);
            await _hotelDbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Owner profile updated successfully",
                ownerId = owner.Id,
                ownerName = owner.Name,
                ownerEmail = owner.Email
            });
        }


    }
}
