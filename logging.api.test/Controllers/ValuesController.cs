using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using logging.api.test.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace logging.api.test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var person = new Person
            {
                age = 29,
                email = "eric.tarelli@gmail.com",
                name = "Eric",
                surname = "Tarelli"
            };

            //_logger.LogInformation($"SERIALIZANDO: {JsonConvert.SerializeObject(person)}");
            _logger.LogInformation("SERIALIZANDO: {@person}", person);

            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
