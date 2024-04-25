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


        /// <summary>
        /// Retrieves all available rooms.
        /// </summary>
        /// <returns>List of all rooms.</returns>
        /// <response code="200">Returns the list of all rooms.</response>
        /// <response code="204">If there are no rooms available.</response>
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

        /// <summary>
        /// Retrieves a specific room by its ID.
        /// </summary>
        /// <param name="id">The ID of the room to retrieve.</param>
        /// <returns>The requested room.</returns>
        /// <response code="200">Returns the requested room.</response>
        /// <response code="404">If the room is not found.</response>
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

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <param name="roomDto">The room data transfer object containing the room's details.</param>
        /// <returns>A newly created room.</returns>
        /// <response code="201">Returns the newly created room.</response>
        /// <response code="400">If the room's details are invalid.</response>
        /// <response code="404">If the owner is not found.</response>
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


        /// <summary>
        /// Deletes a room by its ID.
        /// </summary>
        /// <param name="id">The ID of the room to delete.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="204">If the room is successfully deleted.</response>
        /// <response code="404">If the room is not found.</response>
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

        /// <summary>
        /// Updates the details of an existing room.
        /// </summary>
        /// <param name="roomDto">The room data transfer object containing the updated details.</param>
        /// <param name="id">The ID of the room to update.</param>
        /// <returns>The updated room.</returns>
        /// <response code="200">Returns the updated room.</response>
        /// <response code="400">If the room's new details are invalid.</response>
        /// <response code="404">If the room or owner is not found.</response>
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


        /// <summary>
        /// Assigns a room to a customer for rent.
        /// </summary>
        /// <param name="roomID">The ID of the room to rent.</param>
        /// <param name="customerID">The ID of the customer who is renting the room.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">If the room is successfully rented to the customer.</response>
        /// <response code="400">If the room is not available for rent.</response>
        /// <response code="404">If the room or customer is not found.</response>
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

            return Ok();
        }


    }
}
