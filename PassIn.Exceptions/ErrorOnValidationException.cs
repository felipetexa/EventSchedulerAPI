namespace PassIn.Exceptions;

public class ErrorOnValidationException: SystemException
{
    public ErrorOnValidationException(string message) : base(message)
    {
        
    }
}