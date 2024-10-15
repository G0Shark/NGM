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
        WebClient webClient;
        HttpClient httpClient;
        IConfiguration configurationJs;
        string basePath;
        public Client()
        {
            webClient = new WebClient();
            httpClient = new HttpClient();
            configurationJs = InitConfiguration();
            basePath = configurationJs.GetValue<string>("AppPath");
        }

        IConfiguration InitConfiguration()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("C:/Oleg/git/NGM/NGM_Client/NGM_Client/ClientConfig.json");
            return configurationBuilder.Build();
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

    }
    //httpClient.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("","");
}
