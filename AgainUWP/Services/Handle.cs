using AgainUWP.Emtity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace AgainUWP.Services
{
    class Handle
    {
        public async static Task
Login(string email, string password)
        {
            var tokenJson = await APIHandle.Sign_In(email, password).Result.Content.ReadAsStringAsync();
            Credential tokenResponse = JsonConvert.DeserializeObject<Credential>(tokenJson);
            string jsonUser = JsonConvert.SerializeObject(tokenResponse);
            try
            {
                await WriteFile("credential.txt", jsonUser);
                Debug.WriteLine("Success: " + jsonUser);
            }
            catch
            {
                Debug.WriteLine("Fail: " + jsonUser);
            }
        }
        public async static Task
WriteFile(string file_name, string text)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync(file_name,
                    Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(sampleFile, text);
        }
        public async static Task<string> ReadFile(string file_name)
        {
            Windows.Storage.StorageFolder storageFolder =
                        Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile =
                await storageFolder.GetFileAsync(file_name);
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            return text;
        }
    }
}
