using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LogExtension
{
    [Target("WebAccess")] 
    public class WebAccessTarget: TargetWithLayout
    {
        public WebAccessTarget()
        {
            //this.Host = "localhost";
        }
 
        //[RequiredParameter] 
        //public string Host { get; set; }
 
        protected override void Write(LogEventInfo logEvent) 
        { 
            string logMessage = this.Layout.Render(logEvent); 

            SendTheMessageToRemoteHost(this.Host, logMessage); 
        } 
 
        private void SendTheMessageToRemoteHost(string host, string message) 
        { 
            // TODO - write me 
        } 
    }
}