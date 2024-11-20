// Filters/GlobalExceptionFilter.cs
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using log4net;

namespace Server.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GlobalExceptionFilter));

        public override void OnException(HttpActionExecutedContext context)
        {
            log.Error("Unhandled exception occurred.", context.Exception);
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Произошла ошибка на сервере.");
        }
    }
}
