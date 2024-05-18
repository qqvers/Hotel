using AutoMapper;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Data.Interfaces
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _context;
        private readonly IMapper _mapper;

        public RoomRepository(HotelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            var room = _mapper.Map<Room>(roomDto);

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> UpdateRoomAsync(Guid id, RoomDto roomDto)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return false;

            room = _mapper.Map<Room>(roomDto);

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
