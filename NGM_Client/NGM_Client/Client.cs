using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NGM_Client
{
    class Client
    {
        HttpClient httpClient;
        public Client()
        {
            httpClient = new HttpClient();

        }

        public async Task<string> Ping(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            // выполняем запрос
            HttpResponseMessage response = await httpClient.SendAsync(request);
            string res = await response.Content.ReadAsStringAsync();
            return res;

        }
    }
}
