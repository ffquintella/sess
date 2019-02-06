using System;
using System.ComponentModel.DataAnnotations;

namespace domain
{
    public class SessionRequest
    {
        
        [Required]
        public string App { get; set; }
        
        public short Timeout { get; set; }
        
        [Required]
        public string IpAddress { get; set; }
        
        public string JsonSessionData { get; set; }
    }
}