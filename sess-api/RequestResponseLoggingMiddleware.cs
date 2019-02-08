using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using NLog;

namespace sess_api
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private Logger logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            logger = LogManager.GetLogger("HttpAccessLog");
        }

        public async Task Invoke(HttpContext context)
        {
            logger.Info("URL:{url} Method:{method} From:{ip}", context.Request.GetDisplayUrl(),
                context.Request.Method, context.Connection.RemoteIpAddress);
            await _next(context);
        }

    }
}