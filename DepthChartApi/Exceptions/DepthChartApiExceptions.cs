using System;
using System.Net;
using DepthChartApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DepthChartApi.Exceptions
{
    public class DepthChartApiExceptionFilterAttribute : IActionFilter
    {
        private ILogger<DepthChartApiExceptionFilterAttribute> _logger;
        public DepthChartApiExceptionFilterAttribute(ILogger<DepthChartApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null) return;

            _logger.LogError(context.Exception.Message, context.Exception);

            context.Result = GetUserFriendlyMessageWithStatusCode(context.Exception);
            context.ExceptionHandled = true;
        }

        private ObjectResult GetUserFriendlyMessageWithStatusCode(Exception exception)
        {
            string apiResponseMessage;
            int statusCode;

            if (exception is DepthChartApiException depthChartApiException)
            {
                apiResponseMessage = depthChartApiException.ApiResponseMessage;
                statusCode = depthChartApiException.StatusCode;
            }
            else
            {
                apiResponseMessage = "Api request failed";
                statusCode = 500;
            }

            return new ObjectResult(apiResponseMessage) { StatusCode = statusCode };
        }

        public void OnActionExecuting(ActionExecutingContext context) { }
    }

    public class DepthChartApiException : Exception
    {
        public DepthChartApiException(string message) : base(message) { }
        public DepthChartApiException(string message, Exception innerException) : base(message, innerException) { }

        public virtual string ApiResponseMessage { get; }
        public virtual int StatusCode { get; }
    }

    public class PlayerNotFoundException : DepthChartApiException
    {
        public override string ApiResponseMessage { get => "Player not found with the specified position in the game"; }
        public override int StatusCode => 404;

        public PlayerNotFoundException(string message) : base(message) { }
        public PlayerNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
