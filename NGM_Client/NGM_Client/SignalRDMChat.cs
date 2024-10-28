using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM_Client
{
    class SignalRDMChat: SignalRVoiceChat
    {
        public SignalRDMChat(string _basePath, int _micNumber, string _username = "username", string _password = "password", string _userId = "-1") : base(_basePath, _micNumber, _username, _password, _userId)
        {
            

        }
        new public async Task<string> ConnectToVoiceChat(string _channelId)
        {
            if (_channelId == "" || _channelId == null) return "null channelId";

            channelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/DM")
                .Build();

            hubConnectionVoice.On<string>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message);
            });
            hubConnectionVoice.On<string, string>("ReceiveMessage", async (username, message) =>
            {
                Console.WriteLine(username + ": " + message);
            });
            hubConnectionVoice.On<string, byte[]>("ReceiveVoice", async (_username, data) =>
            {
                OnReceiveVoice(_username, data);
            });



            try
            {
                Console.WriteLine(channelId + " " + username + " " + password);
                await hubConnectionVoice.StartAsync();
                await hubConnectionVoice.InvokeAsync("connect", channelId, username, password);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return ex.Message; }

            micInput.StartRecording();

            return "connected";
        }
    }
}
