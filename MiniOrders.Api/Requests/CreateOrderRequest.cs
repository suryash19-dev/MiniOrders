namespace MiniOrders.Api.Requests;
public class CreateOrderRequest
{
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
}
