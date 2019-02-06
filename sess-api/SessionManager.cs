using System;
using System.Linq;
using NLog;
using sess_api.Tools;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace sess_api
{
    public class SessionManager
    {
        #region SINGLETON

        private static readonly Lazy<SessionManager> lazy = new Lazy<SessionManager>(() => new SessionManager());

        public static SessionManager Instance { get { return lazy.Value; } }

        #endregion
        
        private Logger logger;

        private ConnectionMultiplexer redis;
        
        private SessionManager()
        {
            logger = LogManager.GetCurrentClassLogger();
            
            //var config = ConfigurationManager<>

            var reConf = (IConfigurationSection) VarManager.Instance.Vars["RedisConfig"];

            var servers = reConf.GetSection("servers").Get<string[]>();
            
            logger.Debug("Redis servers: {redis}", JsonConvert.SerializeObject(servers));
             
            //redis = ConnectionMultiplexer.Connect(string.Join(",", servers));
            
        }
        
        
        
        
    }
}