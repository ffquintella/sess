using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NLog;

namespace sess_api.Controllers
{
    public class BaseController: ControllerBase
    {
        protected string requesterID { get; set; }

        protected Logger logger;

        protected IConfiguration configuration;

        protected void ProcessRequest()
        {
            requesterID = this.Request.Headers["api-key"].ToString().Split(':')[0];
        }
    }
}