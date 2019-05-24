using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FantasyCricket.Common.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        internal class ErrorCodes
        {
            public const string UnhandledServiceError = "7150";
            public const string InvalidInputParameter = "7151";
            public const string DestinationHostUnreachableError = "7152";
            public const string DualcomOperationError = "7153";
        }

        internal class HttpFault
        {
            public HttpStatusCode HttpStatusCode { get; set; }
            public string ErrorCode { get; set; }
            public string Message { get; set; }
            public string Resolution { get; set; }
            public string Detail { get; set; }
        }

        public void OnException(ExceptionContext context)
        {
            HttpFault httpFault;

            if (context.Exception is ValidationException)
            {
                httpFault = new HttpFault
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = ErrorCodes.InvalidInputParameter,
                    Message = context.Exception.Message,
                    Resolution = "Correct input parameter",
                    Detail = context.Exception.ToString()
                };
            }



            else  // Unhandled Exception Handler
            {
                httpFault = new HttpFault
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    ErrorCode = ErrorCodes.UnhandledServiceError,
                    Message = string.Format("Unhandled Fantasy Cricket Server error. {0}", context.Exception.Message),
                    Resolution = "Check with fFantasy Cricket support.",
                    Detail = context.Exception.ToString()
                };
            }

            // Send fault response to client
            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)httpFault.HttpStatusCode;

            context.ExceptionHandled = true;

            response.WriteAsync(
                Newtonsoft.Json.JsonConvert.SerializeObject(httpFault, Newtonsoft.Json.Formatting.Indented));
        }
    }
}

