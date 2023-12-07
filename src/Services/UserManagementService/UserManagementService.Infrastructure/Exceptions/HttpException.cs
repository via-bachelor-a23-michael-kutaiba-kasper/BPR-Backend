namespace UserManagementService.Infrastructure.Exceptions;

public class HttpException : Exception
{
    public HttpException(string msg) : base(msg)
    {
    }

    public HttpException() : base()
    {
    }
}