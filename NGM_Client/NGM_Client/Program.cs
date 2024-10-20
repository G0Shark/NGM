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
            Console.WriteLine("userId: ");
            string userId = Console.ReadLine();

            Client client = new Client(userId);


            /*string res = await client.SendRequest(HttpMethod.Put,  "test/put", new List<string> { "TestName", "test_name"});
            Console.WriteLine("put: "+res);
            res = await client.SendRequest(HttpMethod.Post, "test/post", null, new List<string> { "\"Name\"", "\"test_name\"", "\"Value\"","123" });
            Console.WriteLine("post: " + res);
            res = await client.SendRequest(HttpMethod.Post, "test/db", null,null, new List<string> { "Name","test_acc", "password","123","description","qq" });
            Console.WriteLine("test_post: " + res);*/


            
            Console.WriteLine("chatId: ");
            string chatId = Console.ReadLine();
            await client.ConnectToTextChat(chatId);
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();


        }
    }
}
