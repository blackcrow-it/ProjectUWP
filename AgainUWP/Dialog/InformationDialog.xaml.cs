using AgainUWP.Emtity;
using AgainUWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Dialog
{
    public sealed partial class InformationDialog : ContentDialog
    {
        Information information = new Information();
        public InformationDialog()
        {
            GetInfo();
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void GetInfo()
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                Debug.WriteLine(token);
                if (token != "")
                {
                    var httpResponseMessage = APIHandle.GetData(APIUrl.API_INFORMATION, "Basic", token);
                    Debug.WriteLine(httpResponseMessage.Result.StatusCode);
                    //Debug.WriteLine(httpResponseMessage);
                    if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                    {
                        var informationJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                        Debug.WriteLine(HttpStatusCode.Created);
                        information = JsonConvert.DeserializeObject<Information>(informationJson);
                        string[] date = information.birthday.Split('T');
                        this.txt_fullname.Text = (information.firstName + " " + information.lastName).ToUpper();
                        this.txt_birthday.Text = "Birthday: " + date[0];
                        this.txt_email.Text = "Email: " + information.email;
                        this.txt_phone.Text = "Phone: " + information.phone;
                        try
                        {
                            this.img_avatar.ProfilePicture = new BitmapImage(new Uri(information.avatar, UriKind.Absolute));
                        } catch
                        {

                        }
                    }
                }
                Debug.WriteLine(text);
            }
        }
    }
}
