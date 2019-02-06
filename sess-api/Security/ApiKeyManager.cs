using System.IO;
using NLog;
using Newtonsoft.Json;
using System.Collections.Generic;
using domain.Security;
using sess_api.Tools;

namespace sess_api.Security
{
    public static class ApiKeyManager
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static List<ApiKey> keys
        {
            get
            {

                var VarsM = VarManager.Instance;

                if (VarsM.Vars.ContainsKey("ApiKeys"))
                {
                    return (List<ApiKey>) VarsM.Vars["ApiKeys"];
                }
                else
                {
                    var json = File.ReadAllText("security.json");

                    logger.Debug("Json File:" + json);
                    
                    var apiKeys = JsonConvert.DeserializeObject<List<ApiKey>>(json);
                    
                    VarsM.Vars.Add("ApiKeys", apiKeys);

                    return apiKeys;
                }
                
                
            }
        }

        public static ApiKey FindBySecretKey(string secretKey)
        {

            foreach (ApiKey key in keys)
            {
                if (key.secretKey == secretKey) return key;
            }

            return null;
        }

        public static ApiKey Find(string keyID)
        {

            foreach (ApiKey key in keys)
            {
                if (key.keyID == keyID) return key;
            }

            return null;
        }
    }
}