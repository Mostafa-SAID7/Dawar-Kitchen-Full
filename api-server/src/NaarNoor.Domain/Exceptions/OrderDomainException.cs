namespace NaarNoor.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule violation occurs in the Order aggregate.
/// Examples: Invalid order type, insufficient items, invalid status transitions, etc.
/// </summary>
public class OrderDomainException : DomainException
{
    /// <summary>
    /// Creates a new OrderDomainException with the specified message.
    /// </summary>
    public OrderDomainException(string message) : base(message) { }

    /// <summary>
    /// Creates a new OrderDomainException with the specified message and inner exception.
    /// </summary>
    public OrderDomainException(string message, Exception innerException) : base(message, innerException) { }
}
