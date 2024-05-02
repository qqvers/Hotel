using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Data.Interfaces
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _context;

        public RoomRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> GetRoomByIdAsync(Guid id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<Room> AddRoomAsync(RoomDto roomDto)
        {
            var room = new Room
            {
                Name = roomDto.Name,
                Available = roomDto.Available,
                OwnerId = roomDto.OwnerId
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> UpdateRoomAsync(Guid id, RoomDto roomDto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return false;

            room.Name = roomDto.Name;
            room.Available = roomDto.Available;
            room.OwnerId = roomDto.OwnerId;

            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RentRoomAsync(Guid roomId, Guid customerId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || !room.Available)
                return false;

            var customer = await _context.Customers
                .Include(c => c.Rooms)
                .FirstOrDefaultAsync(c => c.Id == customerId);
            if (customer == null)
                return false;

            room.Available = false;
            customer.Rooms.Add(room);

            _context.Update(room);
            _context.Update(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
