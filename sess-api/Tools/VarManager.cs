using System;
using NLog;
using System.Collections.Generic;

namespace sess_api.Tools
{
    public class VarManager
    {
        #region SINGLETON

        private static readonly Lazy<VarManager> lazy = new Lazy<VarManager>(() => new VarManager());

        public static VarManager Instance { get { return lazy.Value; } }

        #endregion
        
        private Logger logger;
        
        private VarManager()
        {
            logger = LogManager.GetCurrentClassLogger();
            Vars = new Dictionary<string, object>();
        }


        public Dictionary<String, Object> Vars { get; set; }


    }
}