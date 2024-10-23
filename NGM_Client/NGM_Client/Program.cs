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

namespace NGM_Client
{

    class Program
    {
        static BufferedWaveProvider bufferStream;
        static WaveInEvent input;
        //поток для речи собеседника
        static WaveOut output;

        
        static byte[] _dat = null;

        static async Task Main(string[] args)
        {
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }


            Console.WriteLine("userId: ");
            string userId = Console.ReadLine();
            Console.WriteLine("chatId: ");
            string chatId = Console.ReadLine();
            Console.WriteLine("micNum: ");
            int micNumber = Convert.ToInt32(Console.ReadLine());

            Client client = new Client(micNumber, userId);


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



            
            await client.ConnectToVoiceChat(chatId);
            Console.ReadLine();
            await client.LeaveVoiceChat();
                        

            Console.ReadLine();



        }


    }
}
