using AutoMapper;
using FakeItEasy;
using hotel_backend.API.Controllers;
using hotel_backend.API.Data.Interfaces;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotel_backend.Tests.Controller
{
    public class OwnersControllerTests
    {
        private readonly OwnersController _controller;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher<Owner> _passwordHasher;
        private readonly SymmetricSecurityKey _loginKey;
        private readonly IMapper _mapper;

        public OwnersControllerTests()
        {
            _ownerRepository = A.Fake<IOwnerRepository>();
            _passwordHasher = A.Fake<IPasswordHasher<Owner>>();
            _loginKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SecretKeySecretKeySecretKeySecretKey"));
            _mapper = A.Fake<IMapper>();
            _controller = new OwnersController(_ownerRepository, _passwordHasher,_mapper);
        }

        [Fact]
        public async Task SignUp_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var ownerDto = new OwnerDto { Name = "John", Email = "john@example.com", Password = "password123" };
            var owner = new Owner { Id = Guid.NewGuid(), Name = "John", Email = "john@example.com" };

            A.CallTo(() => _ownerRepository.SignUpOwnerAsync(ownerDto)).Returns(Task.FromResult(owner));

            // Act
            var result = await _controller.SignUp(ownerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Contains("Owner added successfully", createdResult.Value.ToString());
        }

        [Fact]
        public async Task SignUp_ReturnsBadRequest_WhenEmailIsInUse()
        {
            // Arrange
            var ownerDto = new OwnerDto { Name = "John", Email = "john@example.com", Password = "password123" };

            A.CallTo(() => _ownerRepository.SignUpOwnerAsync(ownerDto))
                .Throws(new InvalidOperationException("Email already in use by another owner"));

            // Act
            var result = await _controller.SignUp(ownerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already in use by another owner", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var ownerDto = new OwnerLoginDto { Email = "john@example.com", Password = "password123" };
            var owner = new Owner { Id = Guid.NewGuid(), Name = "John", Email = "john@example.com" };

            A.CallTo(() => _ownerRepository.AuthenticateOwnerAsync(ownerDto.Email, ownerDto.Password)).Returns(Task.FromResult(owner));

            // Act
            var result = await _controller.Login(ownerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }


        [Fact]
        public async Task Login_ReturnsNotFound_WhenCredentialsAreInvalid()
        {
            // Arrange
            var ownerDto = new OwnerLoginDto { Email = "john@example.com", Password = "wrongpassword" };

            A.CallTo(() => _ownerRepository.AuthenticateOwnerAsync(ownerDto.Email, ownerDto.Password)).Returns(Task.FromResult<Owner>(null));

            // Act
            var result = await _controller.Login(ownerDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }


    }
}
