using FantasyCricket.Common.Net.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FantasyCricket.Common.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        internal class HttpFault
        {
            public HttpStatusCode HttpStatusCode { get; set; }
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
                    Message = context.Exception.Message,
                    Resolution = "Correct input parameter",
                    Detail = context.Exception.ToString()
                };
            }

            else if (context.Exception is UserAlreadyExistsException)
            {
                httpFault = new HttpFault
                {
                    HttpStatusCode = HttpStatusCode.Conflict,
                    Message = context.Exception.Message,
                    Resolution = "Use a different UserName",
                    Detail = context.Exception.ToString()
                };
            }
            else if (context.Exception is NotAGangOwnerException)
            {
                httpFault = new HttpFault
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Message = context.Exception.Message,
                    Resolution = "You can only add users to gangs you create",
                    Detail = context.Exception.ToString()
                };
            }

            else  // Unhandled Exception Handler
            {
                httpFault = new HttpFault
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = string.Format("Unhandled Fantasy Cricket Server error. {0}", context.Exception.Message),
                    Resolution = "Check with Fantasy Cricket support.",
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

