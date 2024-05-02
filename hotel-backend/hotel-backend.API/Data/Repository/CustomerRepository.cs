using hotel_backend.API.Data.Interfaces;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Http.ModelBinding;

namespace hotel_backend.API.Data.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly HotelDbContext _hotelDbContext;
        public CustomerRepository(HotelDbContext hotelDbContext)
        {
            _hotelDbContext = hotelDbContext;
        }
        private string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<Customer>();
            return passwordHasher.HashPassword(null, password);
        }

        public async Task<Customer> SignUpCustomer(CustomerDto customerDto)
        {
            var existingCustomer = await _hotelDbContext.Customers
                .AnyAsync(o => o.Email == customerDto.Email);
            if (existingCustomer)
            {
                throw new InvalidOperationException("Email already in use by another customer");
            }

            var customer = new Customer
            {
                Name = customerDto.Name,
                Email = customerDto.Email,
                Password = HashPassword(customerDto.Password)
            };

            _hotelDbContext.Customers.Add(customer);
            await _hotelDbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> LoginCustomer(CustomerDto customerDto)
        {
            var customer = await _hotelDbContext.Customers
                        .SingleOrDefaultAsync(o => o.Email == customerDto.Email);
            return customer;
        }


        public async Task<Customer> FindCustomerByIdAsync(Guid id)
        {
            return await _hotelDbContext.Customers.FindAsync(id);
        }

        public async Task<bool> CheckEmailExistenceAsync(Guid id, string email)
        {
            return await _hotelDbContext.Customers.AnyAsync(o => o.Email == email && o.Id != id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _hotelDbContext.Customers.Update(customer);
            await _hotelDbContext.SaveChangesAsync();
        }
    }
}
