using AgainUWP.Emtity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }
        private void StopMedia(object sender, RoutedEventArgs e)
        {
            media.Stop();
        }
        private void PauseMedia(object sender, RoutedEventArgs e)
        {
            media.Pause();
        }
        private void PlayMedia(object sender, RoutedEventArgs e)
        {
            media.Play();
        }
        private void Media_State_Changed(object sender, RoutedEventArgs e)
        {
            mediaStateTextBlock.Text = media.CurrentState.ToString();
        }
    }
}
