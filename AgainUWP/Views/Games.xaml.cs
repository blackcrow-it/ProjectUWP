using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Games : Page
    {
        public Games()
        {
            this.InitializeComponent();
        }

        private async void Do_Create(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder =  Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync("credential.txt",
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, "");
                txt_create.Text = "Success";
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Views.Navigation));
            } catch
            {
                txt_create.Text = "Fail";
            }
            
        }

        private async void Do_Read(object sender, RoutedEventArgs e)
        {
            try
            {
            Windows.Storage.StorageFolder storageFolder =
                        Windows.Storage.ApplicationData.Current.LocalFolder;
                        Windows.Storage.StorageFile sampleFile =
                            await storageFolder.GetFileAsync("credential.txt");
                string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                txt_read.Text = text;
            }
            catch
            {
                txt_read.Text = "Fail";
            }
            
        }
    }
}
