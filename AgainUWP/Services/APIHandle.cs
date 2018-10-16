using AgainUWP.Emtity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AgainUWP.Services
{
    class APIHandle
    {
        public async static Task<HttpResponseMessage> Sign_Up(Member member)
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(APIUrl.API_REGISTER, content);
            return response.Result;
        }

        public async static Task<HttpResponseMessage> Sign_In(string email, string password)
        {
            Dictionary<String, String> memberLogin = new Dictionary<string, string>();
            memberLogin.Add("email", email);
            memberLogin.Add("password", password);
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(memberLogin), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(APIUrl.API_LOGIN, content);
            Debug.WriteLine(response.Result.StatusCode);
            return response.Result;
        }
        public async static Task<HttpResponseMessage> GetData(string api_url, string scheme, string token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            var response = httpClient.GetAsync(api_url);
            return response.Result;
        }
        public async static Task<string> GetToken()
        {
            string token;
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            if (await localfolder.TryGetItemAsync("credential.txt") != null)
            {
                string text = await CheckCredential();
                Credential credential = JsonConvert.DeserializeObject<Credential>(text);
                token = credential.token;
            } else
            {
                token = "";
            }
            
            return token;
        }
        public async static Task<string> CheckCredential()
        {
            string text;
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            if (await localfolder.TryGetItemAsync("credential.txt") != null)
            {
                text = await Handle.ReadFile("credential.txt");
            }
            else
            {
                text = "";
            }
            return text;
        }
        public async static Task<HttpResponseMessage> Create_Song(Song song, string scheme, string token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            var content = new StringContent(JsonConvert.SerializeObject(song), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(APIUrl.API_SONG, content);
            return response.Result;
        }
    }
}
