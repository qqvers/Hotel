using hotel_backend.API.Data;
using hotel_backend.API.DTO;
using hotel_backend.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hotel_backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly HotelDbContext _hotelDbContext;

        public RoomsController(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _hotelDbContext.Rooms.ToArrayAsync();
            if (rooms.Length == 0)
            {
                return NoContent();
            }
            return Ok(rooms);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetRoomById([FromRoute] Guid id)
        {
            var room = await _hotelDbContext.Rooms.FindAsync(id);
            if (room is null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto createRoomDto)
        {
            if (createRoomDto is null)
            {
                return BadRequest("Body can not be null");
            }

            var ownerExists = await _hotelDbContext.Owners.AnyAsync(o => o.Id == createRoomDto.OwnerId);
            if (!ownerExists)
            {
                return NotFound("Owner with the provided ID does not exist.");
            }

            var room = new Room()
            {
                Name = createRoomDto.Name,
                Available = createRoomDto.Available,
                OwnerId = createRoomDto.OwnerId,
            };

            await _hotelDbContext.Rooms.AddAsync(room);
            await _hotelDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] Guid id)
        {
            var room = await _hotelDbContext.Rooms.FindAsync(id);
            if(room is null)
            {
                return NotFound("Room with provided ID does not exist");
            }
            _hotelDbContext.Rooms.Remove(room);
            await _hotelDbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
