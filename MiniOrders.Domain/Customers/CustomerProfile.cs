using MiniOrders.Domain.Common;

namespace MiniOrders.Domain.Customers;
public class CustomerProfile : Entity
{
    public Guid CustomerId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
