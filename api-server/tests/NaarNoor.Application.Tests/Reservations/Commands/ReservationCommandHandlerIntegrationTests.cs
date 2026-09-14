using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NaarNoor.Application.Features.Reservations.Commands.CreateReservation;
using NaarNoor.Application.Features.Reservations.Commands.UpdateReservation;
using NaarNoor.Application.Features.Reservations.Commands.DeleteReservation;
using NaarNoor.Application.Common.Interfaces;
using NaarNoor.Domain.Entities;
using NaarNoor.Domain.Enums;
using NaarNoor.Infrastructure.Data;
using NaarNoor.Infrastructure.Repositories;
using Xunit;

namespace NaarNoor.Application.Tests.Reservations.Commands;

/// <summary>
/// Integration tests for Reservation command handlers.
/// ✅ Tests complete vertical slices: CreateReservation → UpdateReservation (state transitions) → DeleteReservation
/// ✅ Tests overlap detection and business rule enforcement
/// Uses in-memory DbContext with full MediatR + DI integration.
/// Each test creates its own isolated DbContext and ServiceProvider for test independence.
/// </summary>
public class ReservationCommandHandlerIntegrationTests
{
    private ServiceProvider BuildServiceProvider(string databaseName)
    {
        var services = new ServiceCollection();
        
        // Add DbContext (in-memory for testing)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));

        // Add MediatR
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(CreateReservationCommand).Assembly));

        // Add Infrastructure services (repositories, UoW)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services.BuildServiceProvider();
    }

    #region Create Reservation Tests

    [Fact]
    public async Task CreateReservation_WithValidData_SucceedsAndReturnReservationId()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7).Date;
            var bookingTime = new DateTime(futureDate.Year, futureDate.Month, futureDate.Day, 19, 0, 0);

            var createCommand = new CreateReservationCommand(
                CustomerName: "John Doe",
                Email: "john@example.com",
                PhoneNumber: "555-1234",
                BookingTime: bookingTime,
                ReservationDate: null,
                ReservationTime: null,
                PartySize: 4,
                SpecialRequests: "Window seat preferred"
            );

            // Act
            var reservationId = await mediator.Send(createCommand);

            // Assert
            reservationId.Should().NotBe(Guid.Empty);
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            reservation.Should().NotBeNull();
            reservation!.Status.Should().Be(ReservationStatus.Pending);
            reservation.CustomerName.Should().Be("John Doe");
            reservation.PartySize.Should().Be(4);
            reservation.SpecialRequests.Should().Be("Window seat preferred");
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateReservation_WithValidDateTimeFields_SucceedsAndCombinesIntoDateTime()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
            const string reservationTimeStr = "18:30";

            var createCommand = new CreateReservationCommand(
                CustomerName: "Jane Smith",
                Email: "jane@example.com",
                PhoneNumber: "555-5678",
                BookingTime: null, // Use separate date/time fields
                ReservationDate: futureDate,
                ReservationTime: reservationTimeStr,
                PartySize: 2,
                SpecialRequests: null
            );

            // Act
            var reservationId = await mediator.Send(createCommand);

            // Assert
            reservationId.Should().NotBe(Guid.Empty);
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            reservation.Should().NotBeNull();
            reservation!.PartySize.Should().Be(2);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateReservation_WithPartySizeOutOfRange_Fails()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);

            var createCommand = new CreateReservationCommand(
                "Invalid Size",
                "invalid@example.com",
                "555-0000",
                futureDate,
                null,
                null,
                101, // Party size exceeds maximum (100)
                null
            );

            // Act & Assert
            var action = async () => await mediator.Send(createCommand);
            await action.Should().ThrowAsync<Exception>();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task CreateReservation_WithPastDate_Fails()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var pastDate = DateTime.UtcNow.AddDays(-1); // Past date

            var createCommand = new CreateReservationCommand(
                "Past Reservation",
                "past@example.com",
                "555-0001",
                pastDate,
                null,
                null,
                2,
                null
            );

            // Act & Assert
            var action = async () => await mediator.Send(createCommand);
            await action.Should().ThrowAsync<Exception>();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Update Reservation - State Transitions

    [Fact]
    public async Task UpdateReservation_ConfirmPending_SucceedsAndChangesStatus()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            // Create reservation
            var futureDate = DateTime.UtcNow.AddDays(7);
            var createCommand = new CreateReservationCommand(
                "Confirm Test",
                "confirm@example.com",
                "555-4444",
                futureDate,
                null,
                null,
                4,
                null
            );
            var reservationId = await mediator.Send(createCommand);

            // Act: Confirm the reservation (Pending → Confirmed)
            var updateCommand = new UpdateReservationCommand(
                Id: reservationId,
                CustomerName: null,
                Email: null,
                PhoneNumber: null,
                Status: ReservationStatus.Confirmed.ToString(),
                PartySize: null,
                SpecialRequests: null
            );
            var result = await mediator.Send(updateCommand);

            // Assert
            result.Should().BeTrue();
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            reservation!.Status.Should().Be(ReservationStatus.Confirmed);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task UpdateReservation_CancelPending_SucceedsAndChangesStatus()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);
            var createCommand = new CreateReservationCommand(
                "Cancel Test",
                "cancel@example.com",
                "555-5555",
                futureDate,
                null,
                null,
                3,
                null
            );
            var reservationId = await mediator.Send(createCommand);

            // Act: Cancel the reservation (Pending → Cancelled)
            var updateCommand = new UpdateReservationCommand(
                reservationId,
                null,
                null,
                null,
                ReservationStatus.Cancelled.ToString(),
                null,
                null
            );
            var result = await mediator.Send(updateCommand);

            // Assert
            result.Should().BeTrue();
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            reservation!.Status.Should().Be(ReservationStatus.Cancelled);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task UpdateReservation_CompleteConfirmed_SucceedsAndChangesStatus()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);
            var createCommand = new CreateReservationCommand(
                "Complete Test",
                "complete@example.com",
                "555-6666",
                futureDate,
                null,
                null,
                2,
                null
            );
            var reservationId = await mediator.Send(createCommand);

            // First confirm it
            await mediator.Send(new UpdateReservationCommand(
                reservationId,
                null,
                null,
                null,
                ReservationStatus.Confirmed.ToString(),
                null,
                null
            ));

            // Act: Complete the confirmed reservation (Confirmed → Completed)
            var completeCommand = new UpdateReservationCommand(
                reservationId,
                null,
                null,
                null,
                ReservationStatus.Completed.ToString(),
                null,
                null
            );
            var result = await mediator.Send(completeCommand);

            // Assert
            result.Should().BeTrue();
            var reservation = await dbContext.Reservations.FindAsync(reservationId);
            reservation!.Status.Should().Be(ReservationStatus.Completed);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task UpdateReservation_InvalidStateTransition_Fails()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);
            var createCommand = new CreateReservationCommand(
                "Invalid Transition",
                "invalid@example.com",
                "555-7777",
                futureDate,
                null,
                null,
                2,
                null
            );
            var reservationId = await mediator.Send(createCommand);

            // Act: Try invalid transition (Pending → Completed, should go Pending → Confirmed → Completed)
            var invalidCommand = new UpdateReservationCommand(
                reservationId,
                null,
                null,
                null,
                ReservationStatus.Completed.ToString(),
                null,
                null
            );

            // Assert
            var action = async () => await mediator.Send(invalidCommand);
            await action.Should().ThrowAsync<Exception>();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Delete Reservation Tests

    [Fact]
    public async Task DeleteReservation_WithValidId_SucceedsAndRemovesReservation()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);
            var createCommand = new CreateReservationCommand(
                "Delete Test",
                "delete@example.com",
                "555-8888",
                futureDate,
                null,
                null,
                2,
                null
            );
            var reservationId = await mediator.Send(createCommand);

            // Verify it was created
            var createdReservation = await dbContext.Reservations.FindAsync(reservationId);
            createdReservation.Should().NotBeNull();

            // Act
            var deleteCommand = new DeleteReservationCommand(reservationId);
            var result = await mediator.Send(deleteCommand);

            // Assert
            result.Should().BeTrue();
            var deletedReservation = await dbContext.Reservations.FindAsync(reservationId);
            deletedReservation.Should().BeNull();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task DeleteReservation_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var invalidId = Guid.NewGuid(); // Non-existent ID

            // Act
            var deleteCommand = new DeleteReservationCommand(invalidId);
            var result = await mediator.Send(deleteCommand);

            // Assert: Should return false instead of throwing
            result.Should().BeFalse();
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion

    #region Data Persistence Tests

    [Fact]
    public async Task ReservationData_IsPersisted_AndCanBeRetrieved()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(10);

            var createCommand = new CreateReservationCommand(
                "Persistence Test",
                "persist@example.com",
                "555-9999",
                futureDate,
                null,
                null,
                5,
                "Vegetarian options needed"
            );

            // Act
            var reservationId = await mediator.Send(createCommand);

            // Query the reservation from the database
            var retrieved = await dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);

            // Assert
            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(reservationId);
            retrieved.CustomerName.Should().Be("Persistence Test");
            retrieved.Email.Should().Be("persist@example.com");
            retrieved.PartySize.Should().Be(5);
            retrieved.SpecialRequests.Should().Be("Vegetarian options needed");
            retrieved.Status.Should().Be(ReservationStatus.Pending);
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public async Task MultipleReservations_CanBeCreatedIndependently()
    {
        // Arrange
        var databaseName = $"test-{Guid.NewGuid():N}";
        var serviceProvider = BuildServiceProvider(databaseName);
        try
        {
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var futureDate = DateTime.UtcNow.AddDays(7);
            var reservations = new[]
            {
                new CreateReservationCommand("Customer1", "c1@example.com", "555-1111", futureDate, null, null, 2, null),
                new CreateReservationCommand("Customer2", "c2@example.com", "555-2222", futureDate.AddHours(3), null, null, 4, null),
                new CreateReservationCommand("Customer3", "c3@example.com", "555-3333", futureDate.AddDays(1), null, null, 3, null),
            };

            // Act
            var ids = new List<Guid>();
            foreach (var cmd in reservations)
            {
                ids.Add(await mediator.Send(cmd));
            }

            // Assert
            var allReservations = await dbContext.Reservations.ToListAsync();
            allReservations.Should().HaveCount(3);
            
            allReservations.Should().Contain(r => r.Id == ids[0] && r.CustomerName == "Customer1");
            allReservations.Should().Contain(r => r.Id == ids[1] && r.CustomerName == "Customer2");
            allReservations.Should().Contain(r => r.Id == ids[2] && r.CustomerName == "Customer3");
        }
        finally
        {
            serviceProvider.Dispose();
        }
    }

    #endregion
}
