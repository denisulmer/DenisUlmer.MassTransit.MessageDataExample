using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DenisUlmer.MessageDataExample.Service;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _hostingEnvironment;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    public void OnException(ExceptionContext context)
    {
        ProblemDetails problemDetails = new()
        {
            Title = "The request could not be processed.",
            Detail = $"If you need further assistance please contact us and include the trace identifier in your request.",
            Instance = context.HttpContext.Request.Path,
            Status = StatusCodes.Status500InternalServerError,
        };

        if (context.Exception is OperationCanceledException)
        {
            problemDetails.Title = "An operation was cancelled";
            problemDetails.Detail = "An operation was cancelled because it exceded the configured timeout";
        }

        if (!_hostingEnvironment.IsProduction())
        {
            problemDetails.Extensions.Add("ExceptionMessage", context.Exception.Message);
            problemDetails.Extensions.Add("Exception", context.Exception);
        }

        problemDetails.Extensions.Add("TraceIdentifier", context.HttpContext.TraceIdentifier);

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new JsonResult(problemDetails);
        context.ExceptionHandled = true;
    }
}