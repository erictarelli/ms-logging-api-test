using logging.api.test.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace logging.api.test
{
    public class MiddlewareOne
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareOne> _logger;
        private readonly ICustomLogger _customlogger;

        public MiddlewareOne(RequestDelegate next, ILogger<MiddlewareOne> logger, ICustomLogger customLogger)
        {
            _next = next;
            _logger = logger;
            _customlogger = customLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
                await _customlogger.LogResponse(context);
            }
            catch (Exception ex) when (ex.Source == System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
            {
                _logger.LogError($"ERROR - MiddlewareOne() - {ex.ToString()}");
                throw ex;
            }

        }
    }
}
