using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.DatabaseAccess.Configuration
{
    public class ElasticSearchConfig
    {
        public List<string> ClusterEndpoints { get; set; }
    }
}
