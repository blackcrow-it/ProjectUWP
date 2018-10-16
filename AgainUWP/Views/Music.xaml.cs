using AgainUWP.Emtity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
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
    public sealed partial class Music : Page
    {
        private ObservableCollection<Song> listSong;
        internal ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }
        MediaPlaybackList playbackList = new MediaPlaybackList();
        public Music()
        {
            this.ListSong = new ObservableCollection<Song>();
            this.ListSong.Add(new Song()
            {
                name = "Hong Kong 1",
                description = "Và giờ anh biết chuyện tình mình chẳng còn gì \nKhi nắng xuân sang lời mật ngọt còn thầm thì",
                thumbnail = "https://znews-photo-td.zadn.vn/w1024/Uploaded/izhqv/2018_10_07/vEwP68qh2cKFSo3XwYKYDwhvsxyGwbA5mdTBvSU7.jpg",
                singer = "Nguyễn Trọng Tài",
                author = "Double X",
                link = "https://od.lk/d/ODBfMjI4MDk1NF8/Hongkong1-Official-Version-Nguyen-Trong-Tai-San-Ji-Double-X.mp3"
            });
            this.ListSong.Add(new Song()
            {
                name = "Nến Và Hoa",
                description = "Buccellati lấp lánh ươm lên tay với nét mặt rạng ngời \nMaserati hai cánh ngăn cho em tách biệt đường đời ",
                thumbnail = "https://i.ytimg.com/vi/D164TFHeOcI/maxresdefault.jpg",
                singer = "Rhymatic - Touliver",
                author = "Rhymatic",
                link = "https://od.lk/d/ODBfMjI4MDk1Nl8/Da-Lo-Yeu-Em-Nhieu-Justatee-HOAPROX-Mix.mp3"
            });
            this.InitializeComponent();
            LoadMusic();
            //time_play.Maximum = mediaSimple.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private async void test(object sender, RoutedEventArgs e)
        {
            //StorageFile musicFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Music/Vai-Thang-Sau-JayKii.mp3"));
            //StorageItemContentProperties fileProperties = musicFile.Properties;
            //MusicProperties musicFileProperties = await fileProperties.GetMusicPropertiesAsync();
            //string[] contributingArtistsKey = { "System.Music.Artist" };
            //IDictionary<string, object> contributingArtistsProperty = await musicFileProperties.RetrievePropertiesAsync(contributingArtistsKey);
            //string[] contributingArtists = contributingArtistsProperty["System.Music.Artist"] as string[];
            //foreach (string contributingArtist in contributingArtists)
            //{
            //    Debug.WriteLine(contributingArtist);
            //}
            playbackList.MoveNext();
        }
        private async void LoadMusic()
        {
            playbackList.AutoRepeatEnabled = false;
            mediaSimple.SetPlaybackSource(playbackList);

            await Task.Run(async () =>
            {
                for (int i = 0; i < listSong.Count(); i++)
                {
                    var song = listSong.ElementAt(i);

                    var source = MediaSource.CreateFromUri(new Uri(song.link));
                    source.CustomProperties["TrackIdKey"] = null;
                    source.CustomProperties["TitleKey"] = song.name;
                    source.CustomProperties["AlbumArtKey"] = song.singer;
                    source.CustomProperties["AuthorKey"] = song.author;
                    source.CustomProperties["TrackNumber"] = (uint)(i + 1);
                    playbackList.Items.Add(new MediaPlaybackItem(source));
                    Debug.WriteLine($"[{i}] {song.name} added to playlist");
                }
            });
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan ts = new TimeSpan(0, 2, 0);
        }

        private void mediaSimple_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(mediaSimple.CurrentState);
        }

        private void mediaSimple_MediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Ended");
        }
    }
}
