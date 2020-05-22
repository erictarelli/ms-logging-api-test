using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logging.api.test.Interface
{
    public interface ICustomLogger 
    {
        Task LogResponse(HttpContext context);
        Task LogRequest(HttpContext context);
    }
}
