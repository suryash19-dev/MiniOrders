using MiniOrders.Domain.Common;
using MiniOrders.Domain.Orders;

namespace MiniOrders.Domain.Customers;
public class Customer : Entity
{
    public string Name { get; set; } = string.Empty;
    public CustomerProfile Profile { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
