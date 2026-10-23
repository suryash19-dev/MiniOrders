using MiniOrders.Application.Orders;

namespace MiniOrders.Application.Customers;
public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CustomerProfileDto Profile { get; set; }
    public IReadOnlyList<OrderDto> Orders { get; set; }
}
