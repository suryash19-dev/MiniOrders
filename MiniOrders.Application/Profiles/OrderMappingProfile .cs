using AutoMapper;
using MiniOrders.Application.Orders;
using MiniOrders.Application.Orders.Commands.CreateOrder;
using MiniOrders.Domain.Orders;

namespace MiniOrders.Application.Profiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderCommand, Order>()
                .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()));
        }
    }
}
