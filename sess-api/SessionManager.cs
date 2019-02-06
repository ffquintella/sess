using System;
using System.Linq;
using domain;
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

        private IDatabase redis_db;
        
        private SessionManager()
        {
            logger = LogManager.GetCurrentClassLogger();
            
            //var config = ConfigurationManager<>

            var reConf = (IConfigurationSection) VarManager.Instance.Vars["RedisConfig"];

            var servers = reConf.GetSection("servers").Get<string[]>();

            var database = reConf.GetSection("database").Get<int>();
            
            logger.Debug("Redis servers: {redis}", JsonConvert.SerializeObject(servers));
            logger.Debug("Redis database: {db}", database);

            try
            {
                redis = ConnectionMultiplexer.Connect(string.Join(",", servers));

                redis_db = redis.GetDatabase(database);
                
                logger.Debug("Connected to redis servers.");
            }
            catch (Exception ex)
            {
                logger.Error("Error connecting to redis servers: {ex}", ex.Message);
            }

        }

        public SessionToken CreateNewSession(SessionRequest request)
        {
            logger.Debug("Creating new session");
            
            var rand = new Random();

            var tokenData = rand.Next(10000, 99999).ToString() + request.IpAddress;

            var token = new SessionToken();
            
            token.Hash = Security.HashHelper.getMD5Hash(tokenData);
            
            //redis_db.StringSet("teste","teste", TimeSpan.FromSeconds(20));

            return token;

        }
        
        
    }
}