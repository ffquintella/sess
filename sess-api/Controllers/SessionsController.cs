using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace sess_api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    public class SessionsController: BaseController
    {

        public SessionsController()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }
        
        // GET api/sessions/52323
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            return "value";
        }
        
        // GET api/sessions
        [HttpPost]
        public ActionResult<string> Get([FromBody] SessionRequest request)
        {
            this.ProcessRequest();
            
            logger.Debug("Creating Session request: {json}", JsonConvert.SerializeObject(request));


            var token = new SessionToken();

            token.Ttl = 123;
            token.Hash = "HADSFASHDFasf";

            return Created("tk", token);
        }
    }
}