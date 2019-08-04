using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using NLog;

namespace sess_api.Security
{
  public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        //private readonly IUserService _userService;

        private Logger logger;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

            this.logger = LogManager.GetCurrentClassLogger();
            //_userService = userService;
        }
        

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            var t = await Task.Run( () =>
            {
                          
                if (!Request.Headers.ContainsKey("api-key"))
                    return AuthenticateResult.Fail("Missing api-key Header");
    
                string api_key = Request.Headers["api-key"];
    
                if (api_key != null)
                {
                    string[] vals = api_key.Split(':');

                    logger.Info("Login attempt with api-key:{0}", vals[0]);

                    var key = ApiKeyManager.Find(vals[0]);
    
                    if (key != null && key.secretKey == vals[1] && key.authorizedIP == Request.HttpContext.Connection.RemoteIpAddress.ToString())
                    {
                        logger.Info("Login success for api-key:{0}", vals[0]);
                        const string Issuer = "https://fgv.br";
                        var claims = new List<Claim>();
    
                        claims.Add(new Claim(ClaimTypes.Name, key.keyID, ClaimValueTypes.String, Issuer));
    
                        List<string> tclaims = HttpSecurity.getClaims(key.secretKey);
    
                        foreach (string claim in tclaims)
                        {
                            claims.Add(new Claim(claim, "true", ClaimValueTypes.Boolean));
                        }
    
    
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);
    
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        logger.Error("Api key not found key:{0}", vals[0]);

                        // FAILED
                        return AuthenticateResult.Fail("Invalid api-key or IP address");
                    }
                }
                else
                {
                    // FAILED
                    return AuthenticateResult.Fail("Invalid api-key");
                }
            
            });

            return t;
        }
    }
}