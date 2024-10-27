using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
namespace NGM_Client
{
    class SignalRChat
    {
        HubConnection hubConnectionText;

        List<string> channelsId;
        string userId = "-1";
        string basePath;
        string username, password;

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
        public SignalRChat(string _basePath, string _username = "username", string _password = "password", string _userId = "-1")
        {
            userId = _userId;
            basePath = _basePath;
            username = _username;
            password = _password;
        }


        public async Task ConnectToTextChat(List<string> _channelsId)
        {
            channelsId = _channelsId;
            hubConnectionText = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/chat")
                .Build();



            hubConnectionText.On<string, string>("ReceiveMessage", (athor, message) =>
            {
                if (userId != athor)//условно
                    Console.WriteLine(athor + ": " + message);
            });

            await hubConnectionText.StartAsync();
            foreach (string channelId in channelsId)
                await hubConnectionText.InvokeAsync("connect", channelId, username,password);
        }


        public async Task LeaveTextChat()
        {
            foreach (string channelId in channelsId)
                await hubConnectionText.InvokeAsync("disconnect", channelId, username, password);
            await hubConnectionText.StopAsync();
            channelsId = new List<string>();
        }



        public async Task SendToTextChat(string channelId, string content)
        {
            if (channelId == "-1") return;
            await hubConnectionText.InvokeAsync("SendToTextChat", channelId, username, content);
        }

    }
}
