using FluentValidation;
using MiniOrders.Application.Customers.Commands.CreateCustomer;
using MiniOrders.Application.Exceptions;
using MiniOrders.Tests.TestSupport;

namespace MiniOrders.Tests.Application;

public sealed class CustomerServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_persists_trimmed_customer_and_profile()
    {
        using var database = new TestDatabase();
        var command = new CreateCustomerCommand("  Ada Lovelace  ", "  1 Main Street  ", " 555-0100 ");

        var id = await database.CreateCustomerCommandService().CreateCustomerAsync(command, CancellationToken.None);
        var customer = await database.Context.Customers.FindAsync(id);

        Assert.NotNull(customer);
        Assert.Equal("Ada Lovelace", customer.Name);
        var profile = database.Context.CustomerProfiles.Single(item => item.CustomerId == customer.Id);
        Assert.NotNull(profile);
        Assert.Equal("1 Main Street", profile.Address);
        Assert.Equal("555-0100", profile.PhoneNumber);
        Assert.Equal(customer.Id, profile.CustomerId);
    }

    [Theory]
    [InlineData("", "Street", "Name")]
    [InlineData("   ", "Street", "Name")]
    [InlineData("x", "", "Address")]
    [InlineData("x", "   ", "Address")]
    public async Task CreateCustomerAsync_rejects_empty_required_fields(string name, string address, string expectedProperty)
    {
        using var database = new TestDatabase();
        var command = new CreateCustomerCommand(name, address, "555-0100");

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            database.CreateCustomerCommandService().CreateCustomerAsync(command, CancellationToken.None));

        Assert.Contains(exception.Errors, error => error.PropertyName == expectedProperty);
        Assert.Empty(database.Context.Customers);
    }

    [Theory]
    [InlineData(101, "Name")]
    [InlineData(201, "Address")]
    public async Task CreateCustomerAsync_rejects_values_over_maximum_length(int length, string property)
    {
        using var database = new TestDatabase();
        var name = property == "Name" ? new string('n', length) : "Valid";
        var address = property == "Address" ? new string('a', length) : "Valid address";

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            database.CreateCustomerCommandService().CreateCustomerAsync(
                new CreateCustomerCommand(name, address, "555-0100"), CancellationToken.None));

        Assert.Contains(exception.Errors, error => error.PropertyName == property);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123456789012345678901")]
    public async Task CreateCustomerAsync_rejects_invalid_phone_numbers(string phoneNumber)
    {
        using var database = new TestDatabase();

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            database.CreateCustomerCommandService().CreateCustomerAsync(
                new CreateCustomerCommand("Valid", "Valid address", phoneNumber), CancellationToken.None));

        Assert.Contains(exception.Errors, error => error.PropertyName == "PhoneNumber");
    }

    [Fact]
    public async Task CreateCustomerAsync_accepts_persistence_maximum_field_lengths()
    {
        using var database = new TestDatabase();
        var command = new CreateCustomerCommand(new string('n', 100), new string('a', 200), new string('1', 20));

        var id = await database.CreateCustomerCommandService().CreateCustomerAsync(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_returns_customer_profile_and_orders()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync("Ada");
        var order = await database.AddOrderAsync(customer.Id);

        var result = await database.CreateCustomerQueryService().GetCustomerByIdAsync(customer.Id, CancellationToken.None);

        Assert.Equal(customer.Id, result.Id);
        Assert.Equal("Ada", result.Name);
        Assert.Equal(customer.Profile.Address, result.Profile.Address);
        Assert.Equal(customer.Profile.PhoneNumber, result.Profile.PhoneNumber);
        Assert.Contains(result.Orders, item => item.Id == order.Id);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_throws_not_found_for_unknown_customer()
    {
        using var database = new TestDatabase();
        var id = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            database.CreateCustomerQueryService().GetCustomerByIdAsync(id, CancellationToken.None));

        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public async Task GetAllCustomersAsync_returns_mapped_customers()
    {
        using var database = new TestDatabase();
        await database.AddCustomerAsync("First");
        await database.AddCustomerAsync("Second");

        var result = await database.CreateCustomerQueryService().GetAllCustomersAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, customer => customer.Name == "First");
        Assert.Contains(result, customer => customer.Name == "Second");
    }
}