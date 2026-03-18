using System.Net;
using PharmaVault.Core.Models;

namespace PharmaVault.Core.Exceptions;

public class ErrorExceptionResponse : Exception
{
    public string ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }
    public ExceptionLogDto Details { get; }

    public ErrorExceptionResponse(string errorCode, HttpStatusCode statusCode, ExceptionLogDto details) 
        : base(details.ErrorMessage)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        Details = details;
    }
}