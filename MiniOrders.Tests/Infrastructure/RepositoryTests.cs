using Microsoft.EntityFrameworkCore;
using MiniOrders.Domain.Orders;
using MiniOrders.Tests.TestSupport;

namespace MiniOrders.Tests.Infrastructure;

public sealed class RepositoryTests
{
    [Fact]
    public async Task CustomerRepository_orders_customers_by_name_and_includes_details()
    {
        using var database = new TestDatabase();
        var second = await database.AddCustomerAsync("Zoe", "2 Street", "555-0002");
        var first = await database.AddCustomerAsync("Ada", "1 Street", "555-0001");
        var older = await database.AddOrderAsync(first.Id, new DateTime(2025, 1, 1));
        var newer = await database.AddOrderAsync(first.Id, new DateTime(2026, 1, 1));

        var customers = await database.UnitOfWork.Customers.GetAllWithDetailsAsync(CancellationToken.None);

        Assert.Equal(new[] { "Ada", "Zoe" }, customers.Select(customer => customer.Name));
        var ada = customers[0];
        Assert.Equal("1 Street", ada.Profile.Address);
        Assert.Equal("555-0001", ada.Profile.PhoneNumber);
        Assert.Equal(new[] { newer.Id, older.Id }, ada.Orders.Select(order => order.Id));
        Assert.Equal(second.Id, customers[1].Id);
    }

    [Fact]
    public async Task CustomerRepository_returns_null_for_missing_id_and_checks_existence()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();

        Assert.Null(await database.UnitOfWork.Customers.GetByIdWithDetailsAsync(Guid.NewGuid(), CancellationToken.None));
        Assert.True(await database.UnitOfWork.Customers.ExistsAsync(customer.Id, CancellationToken.None));
        Assert.False(await database.UnitOfWork.Customers.ExistsAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task OrderRepository_filters_by_customer_and_sorts_newest_first()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();
        var other = await database.AddCustomerAsync("Other");
        var older = await database.AddOrderAsync(customer.Id, new DateTime(2025, 1, 1));
        var newer = await database.AddOrderAsync(customer.Id, new DateTime(2026, 1, 1));
        await database.AddOrderAsync(other.Id, new DateTime(2027, 1, 1));

        var filtered = await database.UnitOfWork.Orders.GetByCustomerIdAsync(customer.Id, CancellationToken.None);
        var all = await database.UnitOfWork.Orders.GetAllAsync(CancellationToken.None);

        Assert.Equal(new[] { newer.Id, older.Id }, filtered.Select(order => order.Id));
        Assert.Equal(3, all.Count);
        Assert.Equal(new DateTime(2027, 1, 1), all[0].OrderDate);
    }

    [Fact]
    public void Model_requires_customer_profile_and_order_customer_relationships()
    {
        using var database = new TestDatabase();
        var model = database.Context.Model;
        var customerType = model.FindEntityType(typeof(Domain.Customers.Customer))!;
        var profileType = model.FindEntityType(typeof(Domain.Customers.CustomerProfile))!;
        var orderType = model.FindEntityType(typeof(Order))!;

        var profileForeignKey = Assert.Single(profileType.GetForeignKeys());
        var orderForeignKey = Assert.Single(orderType.GetForeignKeys());
        Assert.True(profileForeignKey.IsRequired);
        Assert.Equal(DeleteBehavior.Cascade, profileForeignKey.DeleteBehavior);
        Assert.True(orderForeignKey.IsRequired);
        Assert.Equal(DeleteBehavior.Cascade, orderForeignKey.DeleteBehavior);
        Assert.Contains(profileType.GetIndexes(), index => index.IsUnique && index.Properties.Any(property => property.Name == "CustomerId"));
        Assert.Equal(100, customerType.FindProperty(nameof(Domain.Customers.Customer.Name))!.GetMaxLength());
        Assert.Equal(200, profileType.FindProperty(nameof(Domain.Customers.CustomerProfile.Address))!.GetMaxLength());
        Assert.Equal(20, profileType.FindProperty(nameof(Domain.Customers.CustomerProfile.PhoneNumber))!.GetMaxLength());
        Assert.Equal(18, orderType.FindProperty(nameof(Order.TotalAmount))!.GetPrecision());
        Assert.Equal(2, orderType.FindProperty(nameof(Order.TotalAmount))!.GetScale());
    }
}