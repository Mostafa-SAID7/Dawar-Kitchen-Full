using System.Threading;
using System.Threading.Tasks;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern: coordinates multiple repositories within a single transaction scope.
/// Exposes all domain aggregates as repositories.
/// Handlers should use this as the primary persistence abstraction.
/// </summary>
public interface IUnitOfWork
{
    IRepository<Reservation> Reservations { get; }
    IRepository<MenuItem> MenuItems { get; }
    IRepository<Chef> Chefs { get; }
    IRepository<ContactInquiry> ContactInquiries { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<User> Users { get; }
    IRepository<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
