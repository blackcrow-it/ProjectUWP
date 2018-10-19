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
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
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
    public sealed partial class RegisterDialog : ContentDialog
    {
        private Member currentMember;
        private StorageFile photo;
        private string currentUploadUrl;
        public RegisterDialog()
        {
            this.currentMember = new Member();
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.currentMember.email = this.box_email.Text;
            this.currentMember.password = this.pwd_password.Password;
            this.currentMember.firstName = this.box_firstname.Text;
            this.currentMember.lastName = this.box_lastname.Text;
            this.currentMember.phone = this.box_email.Text;
            this.currentMember.address = this.box_address.Text;
            this.currentMember.introduction = this.box_introduction.Text;
            this.currentMember.avatar = this.txb_UrlImage.Text;
            var httpResponseMessage = APIHandle.Sign_Up(this.currentMember);
            if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Success");
                await Handle.Login(currentMember.email, currentMember.password);
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Views.ListViewDemo));
            }
            else
            {
                progress1.IsActive = true;
                progress1.Visibility = Visibility.Visible;
                args.Cancel = true;
                email.Text = password.Text = phone.Text = lastName.Text = firstName.Text = avatar.Text = "";

                var errorJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                ErrorResponse errResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorJson);
                foreach (var errorField in errResponse.error.Keys)
                {
                    TextBlock textBlock = this.FindName(errorField) as TextBlock;
                    if (textBlock != null)
                    {
                        textBlock.Text = "*"+errResponse.error[errorField];
                        Debug.WriteLine("'"+errorField+"' : '"+ errResponse.error[errorField]+ "'");
                        textBlock.Visibility = Visibility.Visible;
                        textBlock.Foreground = new SolidColorBrush(Colors.Red);
                        textBlock.FontSize = 10;
                        textBlock.FontStyle = FontStyle.Italic;
                    }
                }
                progress1.IsActive = false;
                progress1.Visibility = Visibility.Collapsed;
            }
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
            LogInDialog login = new LogInDialog();
            await login.ShowAsync();
        }

        private void RadioCheckRender(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            this.currentMember.gender = Int32.Parse(radio.Tag.ToString());
        }

        private void Do_Changed(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.currentMember.birthday = sender.Date.Value.ToString("yyyy-MM-dd");
        }

        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await this.photo.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                //Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                //Debug.WriteLine(u.AbsoluteUri);
                //ImageUrl.Text = u.AbsoluteUri;
                //MyAvatar.Source = new BitmapImage(u);

                string imageUrl = reader2.ReadToEnd();
                Debug.WriteLine(reader2.ReadToEnd());
                img_Avatar.ProfilePicture = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
                txb_UrlImage.Text = imageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        private async void img_Avatar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(500, 500);

            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }

            HttpClient httpClient = new HttpClient();
            currentUploadUrl = await httpClient.GetStringAsync(Services.APIUrl.GET_UPLOAD_TOKEN);
            Debug.WriteLine(currentUploadUrl);
            HttpUploadFile(currentUploadUrl, "myFile", "image/png");
        }

        private void box_firstname_TextChanged(object sender, TextChangedEventArgs e)
        {
            img_Avatar.DisplayName = box_firstname.Text + " " + box_lastname.Text;
        }

        private void box_lastname_TextChanged(object sender, TextChangedEventArgs e)
        {
            img_Avatar.DisplayName = box_firstname.Text + " " + box_lastname.Text;
        }

        private void txb_UrlImage_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                img_Avatar.ProfilePicture = new BitmapImage(new Uri(txb_UrlImage.Text, UriKind.Absolute));
            } catch
            {
                img_Avatar.ProfilePicture = null;
            }
        }
    }
}
