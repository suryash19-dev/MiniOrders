using MiniOrders.Domain.Common;

namespace MiniOrders.Domain.Orders;
public class Order : Entity
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
