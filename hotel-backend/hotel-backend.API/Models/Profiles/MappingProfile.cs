using AutoMapper;
using hotel_backend.API.Models.Domain;
using hotel_backend.API.Models.DTO;

namespace hotel_backend.API.Models.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Customers
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerLoginDto>().ReverseMap();

            //Owners
            CreateMap<Owner, OwnerDto>().ReverseMap();
            CreateMap<Owner, OwnerLoginDto>().ReverseMap();

            //Rooms
            CreateMap<Room, RoomDto>().ReverseMap();
        }
    }
}
