using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using sess_api.Security;
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
        public ActionResult<SessionToken> Get([FromBody] SessionRequest request)
        {
            this.ProcessRequest();
            
            logger.Debug("Creating Session request: {json}", JsonConvert.SerializeObject(request));


            // Verifing if the app informed is present on the list

            var apiKey = ApiKeyManager.Find(this.requesterID);

            if (!apiKey.apps.Contains(request.App))
            {
                logger.Error("App in request is not present in app authorization.");
                return BadRequest();
            }

            var sMan = SessionManager.Instance;

            var token = sMan.CreateNewSession(request);
        
            return Created(this.Request.GetEncodedUrl(), token);
            //return token;
        }
    }
}