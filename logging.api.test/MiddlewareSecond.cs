using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace logging.api.test
{
    public class MiddlewareSecond
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareSecond> _logger;

        public MiddlewareSecond(RequestDelegate next, ILogger<MiddlewareSecond> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (ex.Source == System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
            {
                _logger.LogError($"ERROR - MiddlewareSecond() - {ex.ToString()}");
                throw ex;
            }

        }
    }
}
