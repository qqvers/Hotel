using hotel_backend.API.Data;
using hotel_backend.API.Data.Interfaces;
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
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
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
            var rooms = await _roomRepository.GetAllRoomsAsync();
            if (rooms == null)
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
        public async Task<IActionResult> GetRoomById(Guid id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null)
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
        /// <response code="403">If user is unauthorized.</response>
        [HttpPost("createroom")]
        [Authorize(Policy ="IsOwner")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomDto roomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdRoom = await _roomRepository.AddRoomAsync(roomDto);
            return CreatedAtAction(nameof(GetRoomById), new { id = createdRoom.Id }, createdRoom);
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
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            bool deleted = await _roomRepository.DeleteRoomAsync(id);
            if (!deleted)
            {
                return NotFound("Room with provided ID does not exist");
            }
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
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDto roomDto, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool updated = await _roomRepository.UpdateRoomAsync(id,roomDto);
            if (!updated)
            {
                return NotFound("Room with provided ID does not exist or could not be updated");
            }
            return Ok(updated);
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
        public async Task<IActionResult> RentRoom(Guid roomID, Guid customerID)
        {
            bool rented = await _roomRepository.RentRoomAsync(roomID, customerID);
            if (!rented)
            {
                return BadRequest("Room not available for rent or customer/room not found");
            }
            return Ok("Room rented successfully");
        }


    }
}
