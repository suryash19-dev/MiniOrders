using Microsoft.EntityFrameworkCore;
using MiniOrders.Domain.Customers;
using MiniOrders.Domain.Orders;

namespace MiniOrders.Infrastructure.Persistence;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

            // One-to-one: Customer <-> CustomerProfile
            builder.HasOne(c => c.Profile)
                .WithOne()
                .HasForeignKey<CustomerProfile>(p => p.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many: Customer -> Orders
            builder.HasMany(c => c.Orders)
                .WithOne()
                .HasForeignKey(o => o.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerProfile
        modelBuilder.Entity<CustomerProfile>(builder =>
        {
            builder.ToTable("CustomerProfiles");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Address).IsRequired().HasMaxLength(200);
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.HasIndex(p => p.CustomerId).IsUnique();
        });

        // Orders
        modelBuilder.Entity<Order>(builder =>
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedNever();
            builder.Property(o => o.OrderDate).IsRequired();
            builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });
    }
}