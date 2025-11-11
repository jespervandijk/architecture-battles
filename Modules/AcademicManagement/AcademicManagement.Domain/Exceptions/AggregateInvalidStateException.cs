namespace AcademicManagement.Domain.Exceptions;

public class AggregateInvalidStateException : Exception
{
    public AggregateInvalidStateException(string message) : base(message)
    {
    }

    public AggregateInvalidStateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
