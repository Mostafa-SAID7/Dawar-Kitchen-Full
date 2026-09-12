using Microsoft.EntityFrameworkCore;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Reservation> Reservations { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<Chef> Chefs { get; }
    DbSet<ContactInquiry> ContactInquiries { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
