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
    public class SessionsController : BaseController
    {

        public SessionsController()
        {
            this.logger = LogManager.GetCurrentClassLogger();
        }

        // GET api/sessions/app1
        [HttpGet("{app}")]
        public ActionResult<SessionToken[]> Get(string app)
        {

            if (!AppIsValid(app)) return BadRequest();

            var sMan = SessionManager.Instance;

            var token = sMan.FindAppTokens(app);

            if (token == null) return NotFound();

            return token;
        }

        // GET api/sessions/app1/414E4C2F470B22AF15400B9874A78744
        [HttpGet("{app}/{hash}")]
        public ActionResult<SessionToken> GetHash(string app, string hash, [FromQuery] bool renew = true, [FromQuery] int  ttl = -1)
        {
            if (!AppIsValid(app)) return BadRequest();

            var sMan = SessionManager.Instance;

            var token = sMan.FindToken(app, hash, renew, ttl);

            if (token == null) return NotFound();

            return token;
        }

        // GET api/sessions/app1/414E4C2F470B22AF15400B9874A78744/exists
        [HttpGet("{app}/{hash}/exists")]
        public ActionResult Exists(string app, string hash, [FromQuery] bool renew = true, [FromQuery] int  ttl = -1)
        {

            if (!AppIsValid(app)) return BadRequest();

            var sMan = SessionManager.Instance;

            var exists = sMan.SessionExists(app, hash, renew, ttl);

            if (!exists) return NotFound();

            return Ok();
        }

        // POST api/sessions
        [HttpPost]
        public ActionResult<SessionToken> Get([FromBody] SessionRequest request)
        {

            logger.Debug("Creating Session request: {json}", JsonConvert.SerializeObject(request));

            if (!AppIsValid(request.App)) return BadRequest();

            var sMan = SessionManager.Instance;

            var token = sMan.CreateNewSession(request);

            return Created(this.Request.GetEncodedUrl(), token);
            //return token;
        }

        // DELETE api/sessions/app1/414E4C2F470B22AF15400B9874A78744
        [HttpDelete("{app}/{hash}")]
        public ActionResult Delete(string app, string hash)
        {
            if (!AppIsValid(app)) return BadRequest();

            var sMan = SessionManager.Instance;


            if (sMan.SessionDelete(app, hash)) return Ok();

            return NotFound();
        }

        private bool AppIsValid(string app)
        {
            this.ProcessRequest();
            // Verifing if the app informed is present on the list

            var apiKey = ApiKeyManager.Find(this.requesterID);

            if (!apiKey.apps.Contains(app))
            {
                logger.Error("App in request is not present in app authorization.");
                return false;
            }
            return true;
        }
    }
}