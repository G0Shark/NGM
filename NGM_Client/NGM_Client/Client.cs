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




namespace NGM_Client
{
    class Client
    {

        

        HttpClient httpClient;
        IConfiguration configurationJs;
        string basePath;

        SignalRChat srChat;
        SignalRVoiceChat srVoiceChat;

        public Client(int _micNumber, string _userId)
        {

            httpClient = new HttpClient();
            InitConfiguration("ClientConfig.json");
            basePath = configurationJs.GetValue<string>("AppPath");

            srChat = new SignalRChat(basePath, _userId);
            srVoiceChat = new SignalRVoiceChat(basePath,_micNumber,_userId);
        }

        
        //private
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

        //getters
        public IConfiguration GetJsConfiguration()
        {
            return configurationJs;
        }

        public SignalRChat Chat()
        {
            return srChat;
        }
        public SignalRVoiceChat VoiceChat()
        {
            return srVoiceChat;
        }

        //setters
        public void SetDefaultRequestHeaders(List<string> headers)
        {
            if (headers != null)
                for (int i = 0; i < headers.Count; i += 2) httpClient.DefaultRequestHeaders.Add(headers[i], headers[i + 1]);
        }
        public void SetAuthorizationHeader(string scheme, string parameter)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, parameter);
        }

        //public

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


    }
}
