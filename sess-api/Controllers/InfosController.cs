using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace sess_api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    public class InfosController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"about", "author"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            switch (id)
            {
                case "about":
                    return "Session Controller API";
                    break;
                case "author":
                    return "Felipe Quintella";
                    break;
                default:
                    return "Value invalid";
                    break;
            }
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