namespace PassIn.Exceptions;

public class NotFoundException(string message) : PassInException(message)
{
    // public NotFoundException(string message) : base(message)
    // {
    //     
    // }
}