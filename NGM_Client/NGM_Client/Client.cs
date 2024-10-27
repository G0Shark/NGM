using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NGM_Client
{
    class Client
    {
        string username, password;
        WClient client;

        public Client (string _username, string _password)
        {
            username = _username;
            password = _password;
            client = new WClient(_username, _password);
            client.SetAuthorizationHeader(username,password);
        }

        //getters
        public ref WClient GetWClient()
        {
            return ref client;
        }

        
        public async Task<int> GetPing()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await client.SendRequest(HttpMethod.Get, "ping");
            sw.Stop();
            return sw.Elapsed.Milliseconds;
        }

        public async Task<Chat[]> GetChats()
        {
            Chat[] channels=new Chat[0];
            string data = await client.SendRequest(HttpMethod.Get, "chat/getchats");
            try
            {
                channels = JsonSerializer.Deserialize<Chat[]>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }

            return channels;
        }
        public async Task<Chat> CteateChats(string chatName, string chatDescription= "_", string chatType= "text")
        {
            string data = await client.SendRequest(HttpMethod.Post, "chat/createchat", null, null, new List<string> {
                    "CHAT_NAME", chatName,
                "CHAT_DESCRIPTION", chatDescription ,
                "CHAT_TYPE", chatType});
            try
            {
                return JsonSerializer.Deserialize<Chat>(data);
            }
            catch (Exception ex) { Console.WriteLine(data); return new Chat(); }
            
        }

    }
}
