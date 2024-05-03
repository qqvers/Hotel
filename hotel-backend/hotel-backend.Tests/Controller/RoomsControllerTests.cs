using FakeItEasy;
using hotel_backend.API.Controllers;
using hotel_backend.API.Data.Interfaces;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace hotel_backend.Tests.Controller
{
    public class RoomsControllerTests
    {
        private readonly RoomsController _controller;
        private readonly IRoomRepository _roomRepository;

        public RoomsControllerTests()
        {
            _roomRepository = A.Fake<IRoomRepository>();
            _controller = new RoomsController(_roomRepository);
        }

        [Fact]
        public async Task GetAllRooms_ReturnsNoContent_WhenNoRoomsAvailable()
        {
            A.CallTo(() => _roomRepository.GetAllRoomsAsync()).Returns(Task.FromResult<IEnumerable<Room>>(new List<Room>()));

            var result = await _controller.GetAllRooms();

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllRooms_ReturnsOk_WithRooms()
        {
            var rooms = new List<Room>
            {
                new Room { Id = Guid.NewGuid(), Name = "Room1", Available = true },
                new Room { Id = Guid.NewGuid(), Name = "Room2", Available = false }
            };
            A.CallTo(() => _roomRepository.GetAllRoomsAsync()).Returns(Task.FromResult<IEnumerable<Room>>(rooms));

            var result = await _controller.GetAllRooms();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRooms = Assert.IsAssignableFrom<IEnumerable<Room>>(okResult.Value);
            Assert.Equal(2, returnedRooms.Count());
        }
        [Fact]
        public async Task GetRoomById_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            Guid roomId = Guid.NewGuid();
            A.CallTo(() => _roomRepository.GetRoomByIdAsync(roomId)).Returns(Task.FromResult<Room>(null));

            var result = await _controller.GetRoomById(roomId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetRoomById_ReturnsOk_WithRoom()
        {
            var room = new Room { Id = Guid.NewGuid(), Name = "Deluxe", Available = true };
            A.CallTo(() => _roomRepository.GetRoomByIdAsync(room.Id)).Returns(Task.FromResult(room));

            var result = await _controller.GetRoomById(room.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(room, okResult.Value);
        }

        [Fact]
        public async Task CreateRoom_ReturnsCreatedAtAction_WhenRoomIsCreated()
        {
            var roomDto = new RoomDto { Name = "New Room", Available = true, OwnerId = Guid.NewGuid() };
            var room = new Room { Id = Guid.NewGuid(), Name = roomDto.Name, Available = roomDto.Available, OwnerId = roomDto.OwnerId };

            A.CallTo(() => _roomRepository.AddRoomAsync(roomDto)).Returns(Task.FromResult(room));

            var result = await _controller.CreateRoom(roomDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetRoomById", createdResult.ActionName);
            Assert.Equal(room.Id, ((Room)createdResult.Value).Id);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            Guid roomId = Guid.NewGuid();
            A.CallTo(() => _roomRepository.DeleteRoomAsync(roomId)).Returns(Task.FromResult(false));

            var result = await _controller.DeleteRoom(roomId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNoContent_WhenRoomIsDeleted()
        {
            Guid roomId = Guid.NewGuid();
            A.CallTo(() => _roomRepository.DeleteRoomAsync(roomId)).Returns(Task.FromResult(true));

            var result = await _controller.DeleteRoom(roomId);

            Assert.IsType<NoContentResult>(result);
        }


    }
}
