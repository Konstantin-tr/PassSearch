using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.Parsing.Configuration
{
    public class RabbitMqConfig
    {
        public string VirtualHost { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
