using System.Threading;
using System.Threading.Tasks;
using NaarNoor.Domain.Entities;

namespace NaarNoor.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IRepository<Reservation> Reservations { get; }
    IRepository<MenuItem> MenuItems { get; }
    IRepository<Chef> Chefs { get; }
    IRepository<ContactInquiry> ContactInquiries { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
