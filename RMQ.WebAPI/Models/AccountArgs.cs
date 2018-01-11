using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMQ.WebApi.Models
{
    public class AccountArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public string MessageContent { get; set; }
    }
}