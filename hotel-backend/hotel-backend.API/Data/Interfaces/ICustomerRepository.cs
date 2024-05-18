using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace hotel_backend.API.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> LoginCustomer(CustomerLoginDto customerDto);
        Task<Customer> SignUpCustomer(CustomerDto customerDto);
        Task<Customer> FindCustomerByIdAsync(Guid id);
        Task<bool> CheckEmailExistenceAsync(Guid id, string email);
        Task UpdateCustomerAsync(Customer customer);
    }
}
