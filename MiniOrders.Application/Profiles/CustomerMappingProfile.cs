using AutoMapper;
using MiniOrders.Application.Customers;
using MiniOrders.Application.Customers.Commands.CreateCustomer;
using MiniOrders.Domain.Customers;

namespace MiniOrders.Application.Profiles;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CustomerProfile, CustomerProfileDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name.Trim()))
            .ForMember(d => d.Profile, o => o.MapFrom(s => new CustomerProfile
            {
                Id = Guid.NewGuid(),
                Address = s.Address.Trim(),
                PhoneNumber = s.PhoneNumber.Trim()
            }))
            .ForMember(d => d.Orders, o => o.Ignore())
            .AfterMap((_, d) => d.Profile.CustomerId = d.Id);
    }
}
