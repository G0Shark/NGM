using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.SignalR.Client;


namespace NGM_Client
{
    class Client
    {
        AudioController ac;

        HubConnection hubConnectionText, hubConnectionVoice;

        HttpClient httpClient;
        IConfiguration configurationJs;
        string basePath;

        List<string> channelsId;
        string userId = "-1";


        public Client(string _userId, int _deviceNumber)
        {
            userId = _userId;
            httpClient = new HttpClient();
            InitConfiguration("ClientConfig.json");
            basePath = configurationJs.GetValue<string>("AppPath");
            ac = new AudioController(_deviceNumber);
        }

        void InitConfiguration(string path)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile(path);

            configurationJs= configurationBuilder.Build();
        }
        string CreateStringContent(ref List<string> values)
        {
            string content = "{";
            for (int i = 0; i < values.Count; i += 2) content += (i == 0 ? "" : ",") + values[i] + " : " + values[i + 1];
            content += "}";
            return content;
        }
        void AddHeadersToRequest(ref HttpRequestMessage request, ref List<string> headers)
        {
            if (headers != null)
                for (int i = 0; i < headers.Count; i += 2) request.Headers.Add(headers[i], headers[i + 1]);
        }
        void AddParamsValuesToPath(ref string path, ref List<string> values)
        {
            if (values != null)
                for (int i = 0; i < values.Count; i += 2) path += (i == 0 ? "?" : "&") + values[i] + "=" + values[i + 1];
        }

        public IConfiguration GetJsConfiguration()
        {
            return configurationJs;
        }

        public AudioController GetAudioController()
        {
            return ac;
        }

        public void SetDefaultRequestHeaders(List<string> headers)
        {
            if (headers != null)
                for (int i = 0; i < headers.Count; i += 2) httpClient.DefaultRequestHeaders.Add(headers[i], headers[i + 1]);
        }
        public void SetAuthorizationHeader(string scheme, string parameter)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }
        public async Task<string> SendRequest(HttpMethod method, string path, List<string> headers = null, List<string> contentValues = null, List<string> paramsValues = null, string mediaType= "application/json")
        {
            AddParamsValuesToPath(ref path, ref paramsValues);
            HttpRequestMessage request = new HttpRequestMessage(method, basePath + path);
            AddHeadersToRequest(ref request, ref headers);


            if (contentValues != null)
            request.Content = new StringContent(
            CreateStringContent(ref contentValues),
            Encoding.UTF8,
            mediaType
            );

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }



        public async Task ConnectToTextChat(List<string> _channelsId)
        {
            channelsId = _channelsId;
            hubConnectionText = new HubConnectionBuilder()
                .WithUrl(basePath+"hub/chat")
                .Build();



            hubConnectionText.On<string, string>("ReceiveMessage", (athor, message) =>
            {
                if(userId!= athor)//условно
                Console.WriteLine(athor + ": " + message);
            });

            await hubConnectionText.StartAsync();
            foreach (string channelId in channelsId)
                await hubConnectionText.InvokeAsync("connect", channelId, userId);
        }


        public async Task LeaveTextChat()
        {
            foreach(string channelId in channelsId)
                await hubConnectionText.InvokeAsync("disconnect", channelId, userId);
            await hubConnectionText.StopAsync();
            channelsId = new List<string>();
        }



        public async Task SendToTextChat(string channelId, string content)
        {
            if (channelId == "-1") return;
            await hubConnectionText.InvokeAsync("SendToTextChat", channelId, userId, content);
        }




        public async Task ConnectToVoiceChat(List<string> _channelsId)
        {
            channelsId = _channelsId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/voice")
                .Build();



            hubConnectionVoice.On<string, string>("ReceiveVoice", (_userId, data) =>
            {
                if (userId != _userId)//условно
                    ac.PlayAudio(Convert.FromBase64String(data).ToArray(), 32000);
            });

            await hubConnectionVoice.StartAsync();
            foreach (string channelId in channelsId)
                await hubConnectionVoice.InvokeAsync("connect", channelId, userId);
        }


        public async Task LeaveVoiceChat()
        {
            foreach (string channelId in channelsId)
                await hubConnectionVoice.InvokeAsync("disconnect", channelId, userId);
            await hubConnectionVoice.StopAsync();
            channelsId = new List<string>();
        }
        public async Task SendToVoiceChat(string channelId)
        {
            ac.StartRecording();
            System.Threading.Thread.Sleep(10);
            ac.StopRecording();
            //PlayAudio(audioStream.ToArray(), 32000);

            await hubConnectionVoice.InvokeAsync("SendToVoiceChat", channelId, userId, Convert.ToBase64String(ac.GetAudioStream().ToArray()));
        }



    }
    //httpClient.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("","");
}
