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
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class AddSongDialog : ContentDialog
    {
        private bool validName = false;
        private bool validSinger = false;
        private bool validAuthor = false;
        private bool validLink = false;
        private bool validThumbnail = false;
        public AddSongDialog()
        {
            this.InitializeComponent();
            CheckSubmitEnable();
        }

        private void AddSongDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = true;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            song_name.Text = song_description.Text = song_singer.Text = song_author.Text = song_thumbnail.Text = song_link.Text = "";
            args.Cancel = true;

        }
        private void song_thumbnail_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                thumbnailImage.ProfilePicture = new BitmapImage(new Uri(song_thumbnail.Text, UriKind.Absolute));
            }
            catch
            {
                thumbnailImage.ProfilePicture = null;
            }
            Regex regex = new Regex(@"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*\.(gif|jpg|jpeg|tiff|png))");
            validThumbnail = Handle.Validateinput(song_thumbnail.Text, regex, thumbnail);
        }
        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = true;
            try
            {
                var song = new Song()
                {
                    name = song_name.Text,
                    thumbnail = song_thumbnail.Text,
                    description = song_description.Text,
                    link = song_link.Text,
                    author = song_author.Text,
                    singer = song_singer.Text
                };
                string token = await APIHandle.GetToken();
                var httpResponseMessage = APIHandle.Create_Song(song, "Basic", token);
                if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                {
                    this.Hide();
                    ContentDialog noWifiDialog = new ContentDialog()
                    {
                        Title = "Message",
                        Content = "Upload Success.",
                        CloseButtonText = "Ok",
                    };
                    await noWifiDialog.ShowAsync();                    
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(Views.ListViewDemo));
                }
                else
                {
                    name.Text = author.Text = singer.Text = thumbnail.Text = link.Text = description.Text = "";

                    var errorJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                    ErrorResponse errResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorJson);
                    foreach (var errorField in errResponse.error.Keys)
                    {
                        TextBlock textBlock = this.FindName(errorField) as TextBlock;
                        if (textBlock != null)
                        {
                            textBlock.Text = "*" + errResponse.error[errorField];
                            Debug.WriteLine("'" + errorField + "' : '" + errResponse.error[errorField] + "'");
                            textBlock.Visibility = Visibility.Visible;
                            textBlock.Foreground = new SolidColorBrush(Colors.Red);
                            textBlock.FontSize = 10;
                            //textBlock.FontStyle = FontStyle.Static;
                        }
                    }
                    Debug.WriteLine("Upload Fail");
                    Debug.WriteLine(args.Cancel);
                }
            }
            catch
            {
                Debug.WriteLine("Error");
            }
        }

        private void song_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            validName = Handle.ValidateinputTypeText(song_name.Text, name);
            CheckSubmitEnable();
        }

        private void song_singer_TextChanged(object sender, TextChangedEventArgs e)
        {
            validSinger = Handle.ValidateinputTypeText(song_singer.Text, singer);
            CheckSubmitEnable();
        }

        private void song_author_TextChanged(object sender, TextChangedEventArgs e)
        {
            validAuthor = Handle.ValidateinputTypeText(song_author.Text, author);
            CheckSubmitEnable();
        }

        private void song_link_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex(@"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*\.mp3)");
            validLink = Handle.Validateinput(song_link.Text, regex, link);
            CheckSubmitEnable();
        }
        private void CheckSubmitEnable()
        {
            if (!validAuthor || !validLink || !validSinger || !validThumbnail || !validName)
            {
                IsSecondaryButtonEnabled = false;
            }
            else
            {
                IsSecondaryButtonEnabled = true;
            }
        }
    }
}
