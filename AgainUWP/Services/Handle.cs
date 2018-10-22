using AgainUWP.Emtity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
        public static bool Validateinput (string input, Regex regex, TextBlock textBlock)
        {
            if (input.Length < 1)
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
            else
            {
                if (regex.IsMatch(input))
                {
                    textBlock.Text = "";
                    return true;
                }
                else
                {
                    textBlock.Text = "*Invalid";
                    textBlock.Foreground = new SolidColorBrush(Colors.Red);
                    textBlock.FontSize = 10;
                    textBlock.FontStyle = FontStyle.Italic;
                    return false;
                }
            }
            
        }
        public static bool ValidateinputTypeName (string input, TextBlock textBlock)
        {
            Regex regex = new Regex(@"^[a-zA-Z_ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶ" +
            "ẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợ" +
            "ụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ\\s]+$");
            if (input.Length > 0)
            {
                if (!regex.IsMatch(input))
                {
                    textBlock.Text = "*Don't contain special characters";
                    textBlock.Foreground = new SolidColorBrush(Colors.Red);
                    textBlock.FontSize = 10;
                    textBlock.FontStyle = FontStyle.Italic;
                    return false;
                }
                else
                {
                    textBlock.Text = "";
                    return true;
                }
            }
            else
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
        }
        public static bool ValidateinputTypeEmail(string input, TextBlock textBlock)
        {
            Regex regex = new Regex(@"^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@"
        + "[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$");
            if (input.Length > 0)
            {
                if (!regex.IsMatch(input))
                {
                    textBlock.Text = "*Invalid: example@gmail.com";
                    textBlock.Foreground = new SolidColorBrush(Colors.Red);
                    textBlock.FontSize = 10;
                    textBlock.FontStyle = FontStyle.Italic;
                    return false;
                }
                else
                {
                    textBlock.Text = "";
                    return true;
                }
            }
            else
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
        }
        public static bool ValidateinputTypePassword(string input, TextBlock textBlock)
        {
            if (input.Length > 0)
            {
                if (input.Length < 6 || input.Length > 24)
                {
                    textBlock.Text = "*Must be from 6 to 24 characters";
                    textBlock.Foreground = new SolidColorBrush(Colors.Red);
                    textBlock.FontSize = 10;
                    textBlock.FontStyle = FontStyle.Italic;
                    return false;
                }
                else
                {
                    textBlock.Text = "";
                    return true;
                }
            }
            else
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
        }
        public static bool ValidateinputTypePhone(string input, TextBlock textBlock)
        {
            Regex regex = new Regex(@"^\s*\+?\s*([0-9][\s-]*){10,13}$");
            if (input.Length > 0)
            {
                if (!regex.IsMatch(input))
                {
                    textBlock.Text = "*Must be 10 or 11 numbers";
                    textBlock.Foreground = new SolidColorBrush(Colors.Red);
                    textBlock.FontSize = 10;
                    textBlock.FontStyle = FontStyle.Italic;
                    return false;
                }
                else
                {
                    textBlock.Text = "";
                    return true;
                }
            }
            else
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
        }
        public static bool ValidateinputTypeText(string input, TextBlock textBlock)
        {
            if (input.Length > 0)
            {
                    textBlock.Text = "";
                    return true;
            }
            else
            {
                textBlock.Text = "*Not empty or null";
                textBlock.Foreground = new SolidColorBrush(Colors.Red);
                textBlock.FontSize = 10;
                textBlock.FontStyle = FontStyle.Italic;
                return false;
            }
        }
    }
}
