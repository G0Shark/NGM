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

using System.Media;
using System.Threading;
using NAudio.Wave;
using System.Globalization;

namespace NGM_Client
{

    class Program
    {
        
        static async Task Main(string[] args)
        {
            

            Client client = new Client();

            /*string res = await client.SendRequest(HttpMethod.Put,  "test/put", new List<string> { "TestName", "test_name"});
            Console.WriteLine("put: "+res);
            res = await client.SendRequest(HttpMethod.Post, "test/post", null, new List<string> { "\"Name\"", "\"test_name\"", "\"Value\"","123" });
            Console.WriteLine("post: " + res);
            res = await client.SendRequest(HttpMethod.Post, "test/db", null,null, new List<string> { "Name","test_acc", "password","123","description","qq" });
            Console.WriteLine("test_post: " + res);*/



            /*await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();*/




            /*await client.VoiceChat().ConnectToVoiceChat(chatId);

            string com = "";
            while (com!="exit")
            {
                com=Console.ReadLine();
                if (com.Split()[0] =="set_volume")client.VoiceChat().SetUserVolume(com.Split()[1], float.Parse(com.Split()[2], CultureInfo.InvariantCulture)/2f);
            }

            await client.VoiceChat().LeaveVoiceChat();
                        

            Console.ReadLine();

            */

        }


    }
}
