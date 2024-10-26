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
using System.Net.Mime;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using System.Diagnostics;




namespace NGM_Client
{
    class WClient
    {

        

        HttpClient httpClient;
        JsonWrapper jsonWrapper;
        string basePath;

        SignalRChat srChat;
        SignalRVoiceChat srVoiceChat;

        string username, password;

        public WClient(string _username, string _password)
        {
            username = _username;
            password = _password;

            httpClient = new HttpClient();
            jsonWrapper=new JsonWrapper("ClientConfig.json");
            basePath = jsonWrapper.GetConfigurationJs().GetSection("AppPath").Value;

            srChat = new SignalRChat(basePath, username,password);
            srVoiceChat = new SignalRVoiceChat(basePath, int.Parse(jsonWrapper.GetConfigurationJs().GetSection("MicrophoneIndex").Value),bool.Parse( jsonWrapper.GetConfigurationJs().GetSection("PrintPing").Value), username, password);
        }

        
        //private
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

        public async Task<string> SendRequest(HttpMethod method, string path, JsonObject content = null, List<string> headers = null, List<string> paramsValues = null, string mediaType= "application/json")
        {
            AddParamsValuesToPath(ref path, ref paramsValues);
            HttpRequestMessage request = new HttpRequestMessage(method, basePath + path);
            AddHeadersToRequest(ref request, ref headers);




            if (content != null)
                request.Content = new StringContent(
                content.ToString(),
                Encoding.UTF8,
                mediaType
                );//CreateStringContent(ref contentValues)

            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }


    }
}
