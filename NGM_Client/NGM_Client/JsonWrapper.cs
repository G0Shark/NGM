using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM_Client
{
    internal class JsonWrapper
    {
        IConfiguration configurationJs;

        public JsonWrapper()
        {
            
        }

        public void InitJsonConfig(string pathToFile)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(pathToFile);

            configurationJs = configurationBuilder.Build();
        }

    }
}
