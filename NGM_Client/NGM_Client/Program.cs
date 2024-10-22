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

using NAudio;
using NAudio.Wave;
using System.Media;

namespace NGM_Client
{

    class Program
    {

        static WaveOut waveOut;
        static byte[] bytes = new byte[1600*100];
        static int indx = 0;
        static int sampleRate = 8000; // 8 kHz
        static int channels = 1; // mono


        static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            Console.WriteLine(e.BytesRecorded);

            
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {


                bytes[indx% (1600 * 100)] = e.Buffer[index];


                Console.Write(e.Buffer[index] + " ");
            }
            
        }
        static void test()
        {
            waveOut = new WaveOut(); // or WaveOutEvent()

            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }

            int waveOutDevices = WaveOut.DeviceCount;
            for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                Console.WriteLine("Device {0}: {1}, {2} channels",
                    waveOutDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }

            WaveInEvent waveInStream = new WaveInEvent();
            waveInStream.DeviceNumber = 0;
            waveInStream.DataAvailable += waveIn_DataAvailable;
            
            waveInStream.WaveFormat = new WaveFormat(sampleRate, channels);//
            waveInStream.StartRecording();

            Console.ReadLine();
            waveInStream.StopRecording();

            File.WriteAllBytes("somefile.wav", bytes);

            /*IWaveProvider provider = new RawSourceWaveStream(
                          new MemoryStream(bytes), new WaveFormat(sampleRate, channels));

            waveOut.Init(provider);
            waveOut.Play();*/

        }

        static async Task Main(string[] args)
        {
            /*Console.WriteLine("userId: ");
            string userId = Console.ReadLine();

            Client client = new Client(userId);*/


            /*string res = await client.SendRequest(HttpMethod.Put,  "test/put", new List<string> { "TestName", "test_name"});
            Console.WriteLine("put: "+res);
            res = await client.SendRequest(HttpMethod.Post, "test/post", null, new List<string> { "\"Name\"", "\"test_name\"", "\"Value\"","123" });
            Console.WriteLine("post: " + res);
            res = await client.SendRequest(HttpMethod.Post, "test/db", null,null, new List<string> { "Name","test_acc", "password","123","description","qq" });
            Console.WriteLine("test_post: " + res);*/


            /*Console.WriteLine("chatId: ");
            string chatId = Console.ReadLine();
            await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();*/


            test();
            Console.ReadLine();
        }
    }
}
