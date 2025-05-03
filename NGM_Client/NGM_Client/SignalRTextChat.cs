using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
namespace NGM_Client
{
    class SignalRTextChat
    {
        HubConnection hubConnectionText;

        List<string> channelsId;
        string userId = "-1";
        string basePath;
        string username, password;
        string authHeader;

        bool DirectMessages=false;

        //private

        //getters

        //setters
        public void SetUserId(string _val)
        {
            userId = _val;
        }
        public void SetUsername(string _val)
        {
            username = _val;
        }
        public void SetPassword(string _val)
        {
            password = _val;
        }


        //public
        public SignalRTextChat(string _basePath, string _username = "username", string _password = "password", string _userId = "-1")
        {
            userId = _userId;
            basePath = _basePath;
            username = _username;
            password = _password;
        }
        public void SetAuthHeader(string a_authHeader)
        {
            authHeader = a_authHeader;
        }

        public async Task ConnectToTextChat(List<string> _channelsId, bool dm=false)
        {
            DirectMessages = dm;

            channelsId = _channelsId;
            hubConnectionText = new HubConnectionBuilder()
                .WithUrl(basePath + "hubs/text", options =>
                {
                    options.Headers["Authorization"] = authHeader;
                }
                )
                .Build();


            hubConnectionText.On<UnauthorizedResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });
            hubConnectionText.On<BadRequestResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });
            hubConnectionText.On<ConflictResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });


            hubConnectionText.On<User, string>("ReceiveMessage", (author, message) =>
            {
                if (userId != author.id.ToString())//условно
                    Console.WriteLine(author.username + ": " + message);
            });
            hubConnectionText.On<User, string>("ReceiveUpdateMessage", (author, message) =>
            {
                if (userId != author.id.ToString())//условно
                    Console.WriteLine(author.username + " changed: " + message);
            });
            hubConnectionText.On<User, string>("ReceiveDeleteMessage", (author, message) =>
            {
                
                Console.WriteLine(author.username + " deleted: " + message);
            });

            await hubConnectionText.StartAsync();
            
            foreach (string channelId in channelsId)
                await hubConnectionText.InvokeAsync("Connect", int.Parse(channelId), DirectMessages);
        }


        public async Task LeaveTextChat(string id)
        {

            channelsId.Remove(id);
            await hubConnectionText.InvokeAsync("Disconnect", int.Parse(id));
            await hubConnectionText.StopAsync();
        }



        public async Task SendToTextChat(string channelId, string content, string reply_id="0")
        {
            //if (channelId == "-1") return;
            
            await hubConnectionText.InvokeAsync("Send", int.Parse(channelId), content, int.Parse(reply_id) ,DirectMessages);
        }

    }
}
