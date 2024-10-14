using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace NGM_Client
{

    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("C:/Oleg/git/NGM/NGM_Client/NGM_Client/ClientConfig.json");
            IConfiguration configuration = configurationBuilder.Build();
            string connectionString = configuration.GetConnectionString("AppPath");
            Console.WriteLine(connectionString);

            Client client=new Client();
            string res = await client.Ping(connectionString);
            Console.WriteLine(res) ;
            Console.ReadLine();
        }
    }
}
