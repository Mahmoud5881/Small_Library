using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Small_Library;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception.Message, "Unhandled exception occurred");
        
        var response = new {
            Message = "Something went wrong, please try again later."
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = 500
        };
        
        context.ExceptionHandled = true;        
    }
}