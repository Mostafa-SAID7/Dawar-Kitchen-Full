using System.Threading;
using System.Threading.Tasks;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Infrastructure.Data;

namespace NaarNoor.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IRepository<Reservation>? _reservations;
    private IRepository<MenuItem>? _menuItems;
    private IRepository<Chef>? _chefs;
    private IRepository<ContactInquiry>? _contactInquiries;
    private IRepository<Order>? _orders;
    private IRepository<OrderItem>? _orderItems;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Reservation> Reservations => _reservations ??= new Repository<Reservation>(_context);
    public IRepository<MenuItem> MenuItems => _menuItems ??= new Repository<MenuItem>(_context);
    public IRepository<Chef> Chefs => _chefs ??= new Repository<Chef>(_context);
    public IRepository<ContactInquiry> ContactInquiries => _contactInquiries ??= new Repository<ContactInquiry>(_context);
    public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);
    public IRepository<OrderItem> OrderItems => _orderItems ??= new Repository<OrderItem>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
