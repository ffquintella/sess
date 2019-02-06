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
    public class SessionsController: BaseController
    {
        // GET api/sessions/52323
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            return "value";
        }
    }
}