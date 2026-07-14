namespace TodoYonetim.Api.Exceptions;

public class AppValidationException : Exception
{
    public AppValidationException(string message) : base(message)
    {
    }
}
