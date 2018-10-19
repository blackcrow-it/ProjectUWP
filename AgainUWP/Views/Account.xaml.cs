using AgainUWP.Dialog;
using AgainUWP.Emtity;
using AgainUWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary> 
    public sealed partial class Account : Page
    {
        Information information = new Information();
        public Account()
        {
            this.InitializeComponent();
            GetInfo();
        }
        //public async Task<string> GetToken()
        //{
        //    Windows.Storage.StorageFolder storageFolder =
        //                Windows.Storage.ApplicationData.Current.LocalFolder;
        //    Windows.Storage.StorageFile sampleFile =
        //        await storageFolder.GetFileAsync("credential.txt");
        //    string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
        //    Credential credential = JsonConvert.DeserializeObject<Credential>(text);
        //    string token = credential.token;
        //    return token;
        //}
        //public async void Test()
        //{
        //    var key = "Basic";
        //    var token = "elyXZcmdLKvJXiSXBN5n9xHG1HowA2gZtpEbae1YgFaRS7lovY3IvVCBWEyB6E2X";
        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key, token);
        //    var response = httpClient.GetAsync(APIUrl.API_INFORMATION);
        //    Debug.WriteLine(response.Result.Content.ReadAsStringAsync());
        //    information = JsonConvert.DeserializeObject<Information>(await response.Result.Content.ReadAsStringAsync());
        //    Debug.WriteLine(information.lastName);
        //}
        public async void GetInfo()
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                Debug.WriteLine(token);
                if (token != "")
                {
                    this.login.Visibility = Visibility.Collapsed;
                    this.account.Visibility = Visibility.Visible;
                    var httpResponseMessage = APIHandle.GetData(APIUrl.API_INFORMATION,"Basic", token);
                    var informationJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                    Debug.WriteLine(informationJson);
                    information = JsonConvert.DeserializeObject<Information>(informationJson);
                    Debug.WriteLine(information.lastName);
                    string[] date = information.birthday.Split('T');
                    this.txt_fullname.Text = (information.firstName + " " + information.lastName).ToUpper();
                    this.txt_birthday.Text = date[0];
                    this.txt_email.Text = information.email;
                    //this.txt_address.Text = information.address;
                    //this.txt_intro.Text = information.introduction;
                    this.img_avatar.ImageSource = new BitmapImage(new Uri(information.avatar, UriKind.Absolute));
                }
                Debug.WriteLine(text);
            }
            else
            {
                this.account.Visibility = Visibility.Collapsed;
                this.login.Visibility = Visibility.Visible;
                this.txt_fullname.Text = "...";
                this.txt_birthday.Text = "...";
                this.txt_email.Text = "...";
                //this.txt_address.Text = "...";
                //this.txt_intro.Text = "...";
                //this.txt_gender.Text = "...";
                Debug.WriteLine("Token not exits");
                Debug.WriteLine(text);
            }
        }

        private async void Do_LogOut(object sender, RoutedEventArgs e)
        {
            try
            {
                Handle.WriteFile("credential.txt", "");
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Views.Navigation));
                Debug.WriteLine("Logout");
            }
            catch
            {
                Debug.WriteLine("Error Logout");
            }
        }

        private async void Do_Login(object sender, RoutedEventArgs e)
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                if (token != "")
                {
                    ContentDialog noWifiDialog = new ContentDialog()
                    {
                        Title = "Thông Báo",
                        Content = "Bạn đã đăng nhập. \nVui lòng đăng xuất để đăng nhập tài khoản khác!",
                        CloseButtonText = "Ok"
                    };
                    await noWifiDialog.ShowAsync();
                }
            }
            else
            {
                LogInDialog login = new LogInDialog();
                await login.ShowAsync();
            }
        }

        private async void Do_Register(object sender, RoutedEventArgs e)
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                if (token != "")
                {
                    ContentDialog noWifiDialog = new ContentDialog()
                    {
                        Title = "Thông Báo",
                        Content = "Bạn đã có tài khoản!",
                        CloseButtonText = "Ok"
                    };
                    await noWifiDialog.ShowAsync();
                }
            }
            else
            {
                Debug.WriteLine("name: " + this.Name);
                RegisterDialog register = new RegisterDialog();
                await register.ShowAsync();
            }
        }
    }
}
