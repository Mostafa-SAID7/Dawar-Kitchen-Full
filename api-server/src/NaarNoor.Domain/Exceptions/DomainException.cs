namespace NaarNoor.Domain.Exceptions;

/// <summary>
/// Base class for all domain exceptions.
/// Domain exceptions represent business rule violations that occur within the domain layer.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Creates a new DomainException with the specified message.
    /// </summary>
    protected DomainException(string message) : base(message) { }

    /// <summary>
    /// Creates a new DomainException with the specified message and inner exception.
    /// </summary>
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}
