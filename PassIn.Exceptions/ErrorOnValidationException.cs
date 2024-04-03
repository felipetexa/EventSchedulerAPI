namespace PassIn.Exceptions;

public class ErrorOnValidationException(string message) : PassInException(message)
{
    // public ErrorOnValidationException(string message) : base(message)
    // {
    //     
    // }
}