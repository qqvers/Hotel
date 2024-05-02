using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;

namespace hotel_backend.API.Data.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room> GetRoomByIdAsync(Guid id);
        Task<Room> AddRoomAsync(RoomDto roomDto);
        Task<bool> UpdateRoomAsync(Guid id, RoomDto roomDto);
        Task<bool> DeleteRoomAsync(Guid id);
        Task<bool> RentRoomAsync(Guid roomId, Guid customerId);
    }
}
