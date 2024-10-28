using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
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
        
        public async Task<User> Login()
        {
            User user = null;
            string data = await (await client.SendRequest(HttpMethod.Post, "auth/login")).Content.ReadAsStringAsync();
            //Console.WriteLine(data);
            try
            {
                user = JsonSerializer.Deserialize<User>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }

            return user;
        }

        public async Task<User> Register(JsonObject jo)
        {
            User user = null;
            string data = await (await client.SendRequest(HttpMethod.Post, "auth/register", jo.ToString(), null)).Content.ReadAsStringAsync();
            try
            {
                user = JsonSerializer.Deserialize<User>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }

            return user;
        }



        public async Task<Chat[]> GetChats()
        {
            Chat[] channels=null;
            string data = await (await client.SendRequest(HttpMethod.Get, "chat/getchats")).Content.ReadAsStringAsync();
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
            Chat channel = null;

            string data = await (await client.SendRequest(HttpMethod.Post, "chat/createchat", null, null, new List<string> {
                    "CHAT_NAME", chatName,
                "CHAT_DESCRIPTION", chatDescription ,
                "CHAT_TYPE", chatType})).Content.ReadAsStringAsync();

            try
            {
                channel = JsonSerializer.Deserialize<Chat>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }

            return channel;
        }
        public async Task<string> AddToChat(string chatId, string otherUserId)
        {

            string data = (await client.SendRequest(HttpMethod.Patch, "chat/addusertochat", null, null, new List<string> {
                    "CHAT_ID", chatId,
                "OTHER_USER_ID", otherUserId })).StatusCode.ToString();

            return data;
        }
        public async Task<string> DeleteFromChat(string chatId, string otherUserId)
        {

            string data = (await client.SendRequest(HttpMethod.Delete, "chat/deluserfromchat", null, null, new List<string> {
                    "CHAT_ID", chatId,
                "OTHER_USER_ID", otherUserId })).StatusCode.ToString();

            return data;
        }



        public async Task<Message[]> GetMessages(string chatId)
        {
            Message[] messages = null;

            string data = await(await client.SendRequest(HttpMethod.Get, "messages/messagesfromchat", null, null, new List<string> {
                    "CHAT_ID", chatId })).Content.ReadAsStringAsync();

            try
            {
                messages = JsonSerializer.Deserialize<Message[]>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }

            return messages;
        }

        public async Task<string> DeleteMessage(string msgId)
        {

            string data = (await client.SendRequest(HttpMethod.Delete, "messages/delmsg", null, null, new List<string> {
                    "MSG_ID", msgId })).StatusCode.ToString();

            return data;
        }

        public async Task<string> ModifyMessage(string msgId, string newContent)
        {

            string data = (await client.SendRequest(HttpMethod.Patch, "messages/modifmsg", null, null, new List<string> {
                    "MSG_ID", msgId })).StatusCode.ToString();

            return data;
        }

    }
}
