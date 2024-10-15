using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NGM_Client
{
    class Client
    {
        HttpClient httpClient;
        IConfiguration configurationJs;
        string basePath;
        public Client()
        {
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

        void AddValuesToPath(ref string path, ref List<List<string>> values)
        {
            if(values!=null)
            for (int i = 0; i < values.Count; i++) path += (i == 0) ? "?" : "&" + values[i][0] + "=" + values[i][1];
        }
        void AddHeadersToRequest(ref HttpRequestMessage request, ref List<List<string>> headers)
        {
            if (headers != null)
                foreach (List<string> header in headers) request.Headers.Add(header[0], header[1]);
        }

        public IConfiguration GetJsConfiguration()
        {
            return configurationJs;
        }

        public async Task<string> Get(string path, List<List<string>> headers = null, List<List<string>> values = null)
        {
            AddValuesToPath(ref path, ref values);
             HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, basePath+path);
            AddHeadersToRequest(ref request, ref headers);
             HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }

        public async Task<string> Put(string path, List<List<string>> headers=null, List<List<string>> values = null)
        {
            AddValuesToPath(ref path, ref values);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, basePath + path);
            AddHeadersToRequest(ref request, ref headers);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }

        public async Task<string> Post(string path, List<List<string>> headers = null, List<List<string>> values = null)
        {
            AddValuesToPath(ref path, ref values);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, basePath + path);
            AddHeadersToRequest(ref request, ref headers);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }
        public async Task<string> Delete(string path, List<List<string>> headers = null, List<List<string>> values = null)
        {
            AddValuesToPath(ref path, ref values);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, basePath + path);
            AddHeadersToRequest(ref request, ref headers);
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }
    }
}
