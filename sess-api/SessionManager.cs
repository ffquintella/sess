using System;
using domain;
using NLog;
using sess_api.Tools;
using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        private int redisDbNum;

        private string[] redisServers;

        protected int defaultTtl;
        
        public int DefaultTtl { get { return defaultTtl; } }
        
        private SessionManager()
        {
            logger = LogManager.GetCurrentClassLogger();
            
            //var config = ConfigurationManager<>

            var reConf = (IConfigurationSection) VarManager.Instance.Vars["RedisConfig"];

            var parameters = (IConfigurationSection)VarManager.Instance.Vars["Parameters"];

            defaultTtl = parameters.GetSection("defaultTTL").Get<int>();

            redisServers = reConf.GetSection("servers").Get<string[]>();

            redisDbNum = reConf.GetSection("database").Get<int>();
            
            logger.Debug("Redis servers: {redis}", JsonConvert.SerializeObject(redisServers));
            logger.Debug("Redis database: {db}", redisDbNum);

            try
            {
                redis = ConnectionMultiplexer.Connect(string.Join(",", redisServers));

                redis_db = redis.GetDatabase(redisDbNum);
                
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

            var rkey = request.App + ":" + token.Hash;

            string val;

            if (request.JsonSessionData != null) val = request.JsonSessionData;
            else val = "";

            int ttl = DefaultTtl;
            if (request.Timeout != 0 && request.Timeout != null) ttl = request.Timeout;

            redis_db.StringSet(rkey,val, TimeSpan.FromSeconds(ttl));

            token.Ttl = ttl;

            return token;

        }

        public SessionToken[] FindAppTokens(string app)
        {
            var tokens = new List<SessionToken>();

            // verify if the token exists in redis


            var server = redis.GetServer(redisServers[0]);


            var keys = server.Keys(redisDbNum, app + ":*");

            if(keys != null)
            {
                foreach(var key in keys)
                {
                    //var val = redis_db.StringGet(key);

                    var token = new SessionToken();

                    token.Hash = key.ToString().Split(':')[1];
                    token.Ttl = (int)redis_db.KeyTimeToLive(key).Value.TotalSeconds;

                    tokens.Add(token);

                }
            }

            return tokens.ToArray();
        }

        public SessionData FindToken(string app, string sessionHash)
        {
            SessionData sdata = null;

            // verify if the token exists in redis

            var key = app + ":" + sessionHash;
                            
            var keyValue = redis_db.StringGet(key);

            if(!keyValue.IsNull)
            {
                sdata = new SessionData();
                sdata.Hash = sessionHash;
                sdata.Data = keyValue.ToString();
                sdata.Ttl = (int)redis_db.KeyTimeToLive(key).Value.TotalSeconds;
                
            }

            return sdata;
        }


    }
}