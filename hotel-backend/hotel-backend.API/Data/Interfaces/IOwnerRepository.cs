using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;

namespace hotel_backend.API.Data.Interfaces
{
    public interface IOwnerRepository
    {
        Task<Owner> SignUpOwnerAsync(OwnerDto ownerDto);
        Task<Owner> AuthenticateOwnerAsync(string email, string password);
        Task<Owner> FindOwnerByIdAsync(Guid id);
        Task<bool> IsEmailInUseAsync(Guid id, string email);
        Task UpdateOwnerAsync(Owner owner);
    }
}
