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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
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
        private bool validEmail = false;
        private bool validPassword = false;
        public LogInDialog()
        {
            this.InitializeComponent();
            CheckSubmitEnable();
        }
        
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var tbemail = tb_email.Text;
            var pbpassword = pb_pass.Password;
            email.Text = password.Text = errorMessage.Text = "";
            var httpResponseMessage = APIHandle.Sign_In(tbemail, pbpassword);
            if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
            {
                await Handle.Login(tbemail, pbpassword);
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Views.ListViewDemo));
            }
            else
            {
                var errorJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                ErrorResponse errResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorJson);
                foreach (var errorField in errResponse.error.Keys)
                {
                    if (this.FindName(errorField) is TextBlock textBlock)
                    {
                        textBlock.Text = "*" + errResponse.error[errorField];
                        Debug.WriteLine("'" + errorField + "' : '" + errResponse.error[errorField] + "'");
                        textBlock.Visibility = Visibility.Visible;
                        textBlock.Foreground = new SolidColorBrush(Colors.Red);
                        textBlock.FontSize = 10;
                        textBlock.FontStyle = FontStyle.Italic;
                    }
                }
                args.Cancel = true;
            }
            if(rememberCheck.IsChecked == true)
            {
                await Handle.WriteFile("remember.txt", "true");
            } else
            {
                await Handle.WriteFile("remember.txt", "");
            }
            //await Task.Delay(1000);
        }

        private async void ChangePage_Register(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegisterDialog register = new RegisterDialog();
            await register.ShowAsync();
        }

        private void tb_email_TextChanged(object sender, TextChangedEventArgs e)
        {
            validEmail = Handle.ValidateinputTypeEmail(tb_email.Text, email);
            CheckSubmitEnable();
        }

        private void pb_pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            validPassword = Handle.ValidateinputTypePassword(pb_pass.Password, password);
            CheckSubmitEnable();
        }
        private void CheckSubmitEnable()
        {
            if (!validEmail || !validPassword)
            {
                IsPrimaryButtonEnabled = false;
            }
            else
            {
                IsPrimaryButtonEnabled = true;
            }
        }
    }
}
