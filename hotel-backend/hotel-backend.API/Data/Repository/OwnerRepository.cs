using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Data.Interfaces
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly HotelDbContext _hotelDbContext;
        private readonly IPasswordHasher<Owner> _passwordHasher;

        public OwnerRepository(HotelDbContext hotelDbContext, IPasswordHasher<Owner> passwordHasher)
        {
            _hotelDbContext = hotelDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Owner> SignUpOwnerAsync(OwnerDto ownerDto)
        {
            var existingOwner = await _hotelDbContext.Owners.AnyAsync(o => o.Email == ownerDto.Email);
            if (existingOwner)
            {
                throw new InvalidOperationException("Email already in use by another owner");
            }

            var owner = new Owner
            {
                Name = ownerDto.Name,
                Email = ownerDto.Email,
                Password = _passwordHasher.HashPassword(null, ownerDto.Password),
                Rooms = new List<Room>()
            };

            _hotelDbContext.Owners.Add(owner);
            await _hotelDbContext.SaveChangesAsync();
            return owner;
        }

        public async Task<Owner> AuthenticateOwnerAsync(string email, string password)
        {
            var owner = await _hotelDbContext.Owners.SingleOrDefaultAsync(o => o.Email == email);
            if (owner == null || _passwordHasher.VerifyHashedPassword(owner, owner.Password, password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return owner;
        }

        public async Task<Owner> FindOwnerByIdAsync(Guid id)
        {
            return await _hotelDbContext.Owners.FindAsync(id);
        }

        public async Task<bool> IsEmailInUseAsync(Guid id, string email)
        {
            return await _hotelDbContext.Owners.AnyAsync(o => o.Email == email && o.Id != id);
        }

        public async Task UpdateOwnerAsync(Owner owner)
        {
            _hotelDbContext.Owners.Update(owner);
            await _hotelDbContext.SaveChangesAsync();
        }
    }


}
