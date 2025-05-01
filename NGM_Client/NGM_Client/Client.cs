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
            client.SetAuthorizationHeader("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password)));
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


        //UserController
        public async Task<User> Login()
        {
            User user = null;
            string data = await (await client.SendRequest(HttpMethod.Get, "auth/login")).Content.ReadAsStringAsync();
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

        public async Task<string> Register(string[] paramsValues)
        {
            User user = null;
            string data = await (await client.SendRequest(HttpMethod.Post, "auth/register", null, null, paramsValues)).Content.ReadAsStringAsync();
            /*try
            {
                user = JsonSerializer.Deserialize<User>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }*/

            return data;
        }


        //ChatController
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

            string data = await (await client.SendRequest(HttpMethod.Post, "chat/createchat", null, null, new string[] {
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

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Patch, "chat/addusertochat", null, null, new string[] {
                    "CHAT_ID", chatId,
                "OTHER_USER_ID", otherUserId }));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }
        public async Task<string> DeleteFromChat(string chatId, string otherUserId)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "chat/deluserfromchat", null, null, new string[] {
                    "CHAT_ID", chatId,
                "OTHER_USER_ID", otherUserId }));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }


        //MessageController
        public async Task<Message[]> GetMessages(string chatId, bool dmChat)
        {
            Message[] messages = null;

            string data = await(await client.SendRequest(HttpMethod.Get, "messages/get", null, null, [
                    "ID", chatId, "DMCHAT",  dmChat?"true":"false"])).Content.ReadAsStringAsync();

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


        //PartnershipController
        public async Task<string> FriendshipAction(FriendshipActions action, string otherUserId)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Post, "friendship/"+ action, null, null, [
                    "RECEIVERID", otherUserId]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }
    }
}
