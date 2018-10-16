using AgainUWP.Emtity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AgainUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string CurrentTag = "";
        public MainPage()
        {
            this.InitializeComponent();
            //this.MyFrame.Navigate(typeof(Views.BlankPage1));
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (CurrentTag == radio.Tag.ToString())
            {
                return;
            }
            switch (radio.Tag.ToString())
            {
                case "Account":
                    CurrentTag = "Account";
                    this.MyFrame.Navigate(typeof(Views.Account));
                    break;
                case "Register":
                    CurrentTag = "Register";
                    this.MyFrame.Navigate(typeof(Views.Register));
                    break;
                case "Music":
                    CurrentTag = "Music";
                    this.MyFrame.Navigate(typeof(Views.Music));
                    break;
                default:
                    break;
            }
        }

        private void Toggle_menu(object sender, RoutedEventArgs e)
        {
            this.SplitViews.IsPaneOpen = !this.SplitViews.IsPaneOpen;
        }
    }
}
