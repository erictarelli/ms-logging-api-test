using logging.api.test.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace logging.api.test
{
    public class MiddlewareThird
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareThird> _logger;
        private readonly ICustomLogger _customlogger;

        public MiddlewareThird(RequestDelegate next, ILogger<MiddlewareThird> logger, ICustomLogger customLogger)
        {
            _next = next;
            _logger = logger;
            _customlogger = customLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _customlogger.LogRequest(context);
                await _next(context);
            }
            catch (Exception ex) when (ex.Source == System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
            {
                _logger.LogError($"ERROR - MiddlewareThird() - {ex.ToString()}");
                throw ex;
            }

        }
    }
}
