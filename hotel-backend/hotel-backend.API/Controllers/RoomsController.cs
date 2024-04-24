using hotel_backend.API.Data;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace hotel_backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly HotelDbContext _hotelDbContext;

        public RoomsController(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }

        [HttpGet("allrooms")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _hotelDbContext.Rooms.ToArrayAsync();
            if (rooms.Length == 0)
            {
                return NoContent();
            }
            return Ok(rooms);
        }

        [HttpGet("room/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoomById([FromRoute] Guid id)
        {
            var room = await _hotelDbContext.Rooms.FindAsync(id);
            if (room is null)
            {
                return NotFound();
            }
            return Ok(room);
        }

        [HttpPost("createroom")]
        [Authorize(Policy ="IsOwner")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDto roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerExists = await _hotelDbContext.Owners.AnyAsync(o => o.Id == roomDto.OwnerId);
            if (!ownerExists)
            {
                return NotFound("Owner with the provided ID does not exist.");
            }

            var room = new Room()
            {
                Name = roomDto.Name,
                Available = roomDto.Available,
                OwnerId = roomDto.OwnerId,
            };

            await _hotelDbContext.Rooms.AddAsync(room);
            await _hotelDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);

        }

        [HttpDelete("deleteroom/{id}")]
        [Authorize(Policy = "IsOwner")]
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

        [HttpPut("update/{id}")]
        [Authorize(Policy = "IsOwner")]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDto roomDto, [FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerExists = await _hotelDbContext.Owners.AnyAsync(o => o.Id == roomDto.OwnerId);
            if (!ownerExists)
            {
                return NotFound("Owner with the provided ID does not exist.");
            }

            var room = new Room()
            {
                Id = id,
                Name = roomDto.Name,
                Available = roomDto.Available,
                OwnerId = roomDto.OwnerId,
            };

            _hotelDbContext.Rooms.Update(room);
            await _hotelDbContext.SaveChangesAsync();

            return Ok(room);

        }

        [HttpPut("rentroom/{roomID}/{customerID}")]
        [Authorize(Policy = "IsCustomer")]
        public async Task<IActionResult> RentRoom([FromRoute] Guid roomID, [FromRoute] Guid customerID)
        {
            var room = await _hotelDbContext.Rooms.FindAsync(roomID);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            if (!room.Available)
            {
                return BadRequest("Room is not available for rent.");
            }


            var customer = await _hotelDbContext.Customers
                .Include(c => c.Rooms) 
                .FirstOrDefaultAsync(c => c.Id == customerID);
            if (customer is null)
            {
                return NotFound("Customer not found.");
            }

            room.Available = false; 
            room.CustomerId = customerID;
            customer.Rooms.Add(room);

            _hotelDbContext.Rooms.Update(room);
            _hotelDbContext.Customers.Update(customer);
            await _hotelDbContext.SaveChangesAsync();

            return Ok(new { Message = "Room rented successfully" });
        }


    }
}
