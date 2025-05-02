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


        //Auth
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

        public async Task<string> Delete()
        {
            string data = await (await client.SendRequest(HttpMethod.Delete, "auth/delete")).Content.ReadAsStringAsync();
            //Console.WriteLine(data);

            return data;
        }

        public async Task<string> AuthChange(AuthParams param, string value)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Post, "auth/" + param, null, null, [
                    ("" + param).ToUpper(), value]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<User> AuthChange(string username)
        {

            string data = await (await client.SendRequest(HttpMethod.Get, "auth", null, null, [
                    "USERNAME", username])).Content.ReadAsStringAsync();
            User user = null;
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
        public async Task<Chat> GetChat()
        {
            Chat channel=null;
            string data = await (await client.SendRequest(HttpMethod.Get, "chat")).Content.ReadAsStringAsync();
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
        public async Task<string> CteateChat(string serverId,string chatName, string chatDescription= "_", string chatType= "text", string rolesId="0")
        {
            //Chat channel = null;

            HttpResponseMessage data =  (await client.SendRequest(HttpMethod.Post, "chat/create", null, null, new string[] {
                "SERVERID", serverId,
                    "NAME", chatName,
                "DESCRIPTION", chatDescription ,
                "TYPE", chatType,
                "ROLEID", rolesId
            }));

            /*
            try
            {
                channel = JsonSerializer.Deserialize<Chat>(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(data);
            }*/

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteChat(string id)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "chat/delete", null, null, new string[] {
                "ID", id
            }));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
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



        //Roles
        public async Task<string> RoleCreate(string serverId, string level, string permission, string name)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Post, "roles/create", null, null, [
                    "SERVERID", serverId,
                    "LEVEL", level,
                    "PERMISSIONS", permission,
                    "NAME", name,

            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<string> RoleDelete(string id)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "roles/delete", null, null, [
                    "ID", id
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        
        public async Task<string> RoleChange(string id, RoleParams param, string value)
        {
            string pName = "" + param;
            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Patch, "roles/"+ param, null, null, [
                    "ID", id,
                    pName.ToUpper(), value
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }




        public async Task<string> ServerCreate(string name, string description, string joinpassword, bool visible)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Post, "servers/create", null, null, [
                    "NAME", name,
                    "DESCRIPTION", description,
                    "JOINPASSWORD", joinpassword,
                    "VISIBLE", visible?"true":"false",

            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<string> ServerDelete(string id)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "servers/delete", null, null, [
                    "ID", id
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }


        public async Task<string> ServerChange(string id, ServerParams param, string value)
        {
            string pName = ""+param;

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Patch, "servers/" + param, null, null, [
                    "ID", id,
                    pName.ToUpper(), value
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }



        //Members

        public async Task<string> MemberJoin(string serverId, string joinPassword)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "servers/delete", null, null, [
                    "SERVERID", serverId,
                    "JOINPASSWORD", joinPassword
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<string> MemberLeave(string serverId)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "servers/delete", null, null, [
                    "SERVERID", serverId
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

        public async Task<string> MemberMute(MuteParams mode, string userId, string serverId, bool value)
        {

            HttpResponseMessage data = (await client.SendRequest(HttpMethod.Delete, "servers/"+ mode, null, null, [
                    "USERID", userId,
                    "SERVERID", serverId,
                    (""+mode).ToUpper(),value?"true":"false"
            ]));

            return data.StatusCode.ToString() + " " + await data.Content.ReadAsStringAsync();
        }

    }
}
