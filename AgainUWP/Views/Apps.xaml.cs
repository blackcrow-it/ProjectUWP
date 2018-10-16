using AgainUWP.Emtity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
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
    public sealed partial class Apps : Page
    {
        public Apps()
        {
            this.InitializeComponent();
        }

        private async void Do_Save(object sender, RoutedEventArgs e)
        {
            string fileName = NameFile.Text;
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = fileName;
            BaseInformation information = new BaseInformation();
            information.Name = txt_Name.Text;
            information.Email = txt_Email.Text;
            information.Phone = txt_Phone.Text;
            var json = JsonConvert.SerializeObject(information);
            

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, json);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    MessageFile.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    MessageFile.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                MessageFile.Text = "Operation cancelled.";
            }
        }

        private async void Do_Open(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var json = await FileIO.ReadTextAsync(file);
                var information = JsonConvert.DeserializeObject<BaseInformation>(json);
                // Application now has read/write access to the picked file
                this.NameFile.Text = file.Name;
                this.txt_Name.Text = information.Name;
                this.txt_Email.Text = information.Email;
                this.txt_Phone.Text = information.Phone;
            }
            else
            {
                this.MessageFile.Text = "Operation cancelled.";
            }
        }
    }
}
