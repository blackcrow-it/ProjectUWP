using AgainUWP.Emtity;
using AgainUWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Dialog
{
    public sealed partial class LogInDialog : ContentDialog
    {
        public LogInDialog()
        {
            this.InitializeComponent();
        }
        
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var email = tb_email.Text;
            var password = pb_pass.Password;
            errorEmail.Text = errorPassword.Text = errorMessage.Text = "";
            var httpResponseMessage = APIHandle.Sign_In(email, password);
            if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
            {
                Handle.Login(email, password);
            }
            else
            {
                if (tb_email.Text == "" && pb_pass.Password != "")
                {
                    errorEmail.Text = "Email empty";
                }
                else if (tb_email.Text != "" && pb_pass.Password == "")
                {
                    errorPassword.Text = "Pass empty";
                }
                else if (tb_email.Text == "" && pb_pass.Password == "")
                {
                    errorEmail.Text = "Email empty";
                    errorPassword.Text = "Pass empty";
                } else
                {
                    errorMessage.Text = "Email or Password incorrect";
                }
                args.Cancel = true;
            }
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Views.Navigation));
        }

        private async void ChangePage_Register(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegisterDialog register = new RegisterDialog();
            await register.ShowAsync();
        }
    }
}
