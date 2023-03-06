namespace Application.Exceptions;

public class AccessDenied : Exception
{
    public AccessDenied(string message) : base(message)
    {
    }
}