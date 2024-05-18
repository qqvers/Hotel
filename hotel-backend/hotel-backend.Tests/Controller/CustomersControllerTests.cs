using AutoMapper;
using FakeItEasy;
using hotel_backend.API.Controllers;
using hotel_backend.API.Data.Interfaces;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace hotel_backend.Tests.Controller
{
    public class CustomersControllerTests
    {
        private readonly CustomersController _controller;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomersControllerTests()
        {
            _customerRepository = A.Fake<ICustomerRepository>();
            _mapper = A.Fake<IMapper>();
            _controller = new CustomersController(_customerRepository, _mapper);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsOk_WhenProfileIsUpdatedSuccessfully()
        {
            // Arrange
            var customerDto = new CustomerDto { Name = "John Doe Updated", Email = "johndoe@example.com", Password = "NewPassword123" };
            var customer = new Customer { Id = Guid.NewGuid(), Email = "johndoe@example.com", Name = "John Doe" };

            A.CallTo(() => _customerRepository.FindCustomerByIdAsync(customer.Id)).Returns(Task.FromResult(customer));
            A.CallTo(() => _customerRepository.CheckEmailExistenceAsync(customer.Id, customerDto.Email)).Returns(Task.FromResult(false));
            A.CallTo(() => _customerRepository.UpdateCustomerAsync(A<Customer>.That.Matches(c => c.Email == customerDto.Email))).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCustomer(customer.Id, customerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerDto = new CustomerDto { Name = "John Doe Updated", Email = "johndoe@example.com", Password = "NewPassword123" };
            Guid customerId = Guid.NewGuid();

            A.CallTo(() => _customerRepository.FindCustomerByIdAsync(customerId)).Returns(Task.FromResult<Customer>(null));

            // Act
            var result = await _controller.UpdateCustomer(customerId, customerDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SignUpCustomer_ReturnsOk_WhenCustomerIsAddedSuccessfully()
        {
            // Arrange
            var customerDto = new CustomerDto { Name = "Jane Doe", Email = "janedoe@example.com", Password = "Password123" };
            var customer = new Customer { Id = Guid.NewGuid(), Name = "Jane Doe", Email = "janedoe@example.com" };

            A.CallTo(() => _customerRepository.SignUpCustomer(customerDto)).Returns(Task.FromResult(customer));

            // Act
            var result = await _controller.SignUpCustomer(customerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Contains("User added successfully", createdResult.Value.ToString());
        }

        [Fact]
        public async Task SignUpCustomer_ReturnsBadRequest_WhenEmailIsAlreadyUsed()
        {
            // Arrange
            var customerDto = new CustomerDto { Name = "John Doe", Email = "existing@example.com", Password = "Password123" };

            A.CallTo(() => _customerRepository.SignUpCustomer(customerDto)).Throws(new InvalidOperationException("Email already in use by another customer"));

            // Act
            var result = await _controller.SignUpCustomer(customerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already in use by another customer", badRequestResult.Value.ToString());
        }


    }


}
