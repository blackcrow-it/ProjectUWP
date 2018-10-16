using AgainUWP.Emtity;
using AgainUWP.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AgainUWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewDemo : Page
    {
        private bool isPlaying = false;
        private bool isVolume = true;
        private double valueVolume = 100;
        private double lastValueVolume;
        private int indexSong = 0;
        private string iconVolume = "\uE995";
        TimeSpan _position;
        DispatcherTimer _timer = new DispatcherTimer();
        private ObservableCollection<Song> listSong;
        private ObservableCollection<Song> listSongLocal;
        internal ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }
        internal ObservableCollection<Song> ListSongLocal { get => listSongLocal; set => listSongLocal = value; }

        public ListViewDemo()
        {
            this.ListSong = new ObservableCollection<Song>();
            //this.ListSong.Add(new Song()
            //{
            //    name = "Hong Kong 1",
            //    description = "Và giờ anh biết chuyện tình mình chẳng còn gì \nKhi nắng xuân sang lời mật ngọt còn thầm thì",
            //    thumbnail = "https://znews-photo-td.zadn.vn/w1024/Uploaded/izhqv/2018_10_07/vEwP68qh2cKFSo3XwYKYDwhvsxyGwbA5mdTBvSU7.jpg",
            //    singer = "Nguyễn Trọng Tài",
            //    author = "Double X",
            //    link = "https://od.lk/d/ODBfMjI4MDk1NF8/Hongkong1-Official-Version-Nguyen-Trong-Tai-San-Ji-Double-X.mp3"
            //});
            //this.ListSong.Add(new Song()
            //{
            //    name = "Nến Và Hoa",
            //    description = "Buccellati lấp lánh ươm lên tay với nét mặt rạng ngời \nMaserati hai cánh ngăn cho em tách biệt đường đời ",
            //    thumbnail = "https://i.ytimg.com/vi/D164TFHeOcI/maxresdefault.jpg",
            //    singer = "Rhymatic - Touliver",
            //    author = "Rhymatic",
            //    link = "https://od.lk/d/ODBfMjI4MDk1Nl8/Da-Lo-Yeu-Em-Nhieu-Justatee-HOAPROX-Mix.mp3"
            //});
            this.InitializeComponent();
            LoadAllSong(ListSong);
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += TickTock;
            _timer.Start();
            LoadAllSongLocal();
            //LoadSong(ListSong[0]);
            //PlayerMedia.AutoPlay = false;
            //Debug.WriteLine(arraySong.SelectedIndex);
        }

        private async void Do_Add(object sender, RoutedEventArgs e)
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
            
            string token;
            try
            {
                name.Text = description.Text = singer.Text = author.Text = thumbnail.Text = link.Text = "";
                token = await APIHandle.GetToken();
                var httpResponseMessage = APIHandle.Create_Song(song, "Basic", token);
                if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                {
                    ContentDialog noWifiDialog = new ContentDialog()
                    {
                        Title = "Message",
                        Content = "Upload Success.",
                        CloseButtonText = "Ok"
                    };
                    ListSong.Add(song);
                    await noWifiDialog.ShowAsync();
                    song_name.Text = song_description.Text = song_singer.Text = song_author.Text = song_thumbnail.Text = song_link.Text = "";
                }
                else
                {
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
                }
            }
            catch
            {
                Debug.WriteLine("Error");
            }
        }

        private async static void LoadAllSong(ObservableCollection<Song> ListSong)
        {
            string token;
            try
            {
                token = await APIHandle.GetToken();
                var httpResponseMessage = APIHandle.GetData(APIUrl.API_SONG, "Basic", token);
                var informationJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                var SongModel = JsonConvert.DeserializeObject<ObservableCollection<Song>>(informationJson);
                foreach (var data in SongModel)
                {
                    ListSong.Add(data);
                    //Debug.WriteLine(data.name + " : " +data.thumbnail); ;
                }
            }
            catch
            {
                Debug.WriteLine("Error");
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            Song song = grid.Tag as Song;
            indexSong = arraySong.SelectedIndex;
            LoadSong(song);
            PlayerMedia.AutoPlay = true;
            Play_Song();
        }

        private void LoadSong(Emtity.Song currentSong)
        {
            this.name_song.Text = "Loading...";
            try
            {
                PlayerMedia.Source = new Uri(currentSong.link);
                PlayerMedia.AutoPlay = false;
            }
            catch
            {
                PlayerMedia.Source = new Uri("https://mp3.zing.vn/bai-hat/Nen-Va-Hoa-Touliver-Remix-Touliver-Rhymastic/ZW9D89D8.html?play_song=1.mp3");
                //Debug.WriteLine("error load uri");
            }
            this.name_song.Text = currentSong.name;
            this.singer_song.Text = currentSong.singer;
            this.avatar_song.DisplayName = currentSong.singer;
            try
            {
                this.avatar_song.ProfilePicture = new BitmapImage(new Uri(currentSong.thumbnail, UriKind.Absolute));
            }
            catch
            {
                Debug.WriteLine("error uri thumnail");
                this.avatar_song.ProfilePicture = null;
            }
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                Pause_Song();
            }
            else
            {
                Play_Song();
            }
        }
        private void Play_Song()
        {
            PlayerMedia.Play();
            btnPlay.Icon = new SymbolIcon(Symbol.Pause);
            isPlaying = true;
            Debug.WriteLine(arraySong.SelectedIndex);
        }
        private void Pause_Song()
        {
            PlayerMedia.Pause();
            btnPlay.Icon = new SymbolIcon(Symbol.Play);
            isPlaying = false;
        }
        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider vol = sender as Slider;
            if (vol != null)
            {
                PlayerMedia.Volume = vol.Value / 100;
                valueVolume = vol.Value;
            }
            if (vol.Value == 0)
            {
                icon_volume.Glyph = "\uE74F";
                iconVolume = "\uE74F";
            }
            else if (1 <= vol.Value && vol.Value < 33)
            {
                icon_volume.Glyph = "\uE992";
                iconVolume = "\uE992";
            }
            else if (33 <= vol.Value && vol.Value < 66)
            {
                icon_volume.Glyph = "\uE993";
                iconVolume = "\uE993";
            }
            else if (66 <= vol.Value && vol.Value < 99)
            {
                icon_volume.Glyph = "\uE994";
                iconVolume = "\uE994";
            }
            else if (vol.Value == 100)
            {
                icon_volume.Glyph = "\uE995";
                iconVolume = "\uE995";
            }
            isVolume = true;
        }
        private void AutoNext()
        {
            if (btnShuffle.IsChecked == true)
            {
                Random random = new Random();
                indexSong = random.Next(0, (ListSong.Count));
            }
            else
            {
                if (indexSong < ListSong.Count - 1 && indexSong >= 0)
                {
                    indexSong = indexSong + 1;
                }
                else
                {
                    indexSong = 0;
                }
            }
            LoadSong(ListSong[indexSong]);
            if (indexSong == 0)
            {
                time_play.Value = 0;
            }
            PlayerMedia.AutoPlay = true;
            Play_Song();
            arraySong.SelectedIndex = indexSong;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PlayerMedia.Stop();
            if (btnRepeat.IsChecked == null)
            {
                indexSong = indexSong;
            }
            else
            {
                if (btnShuffle.IsChecked == true)
                {
                    Random random = new Random();
                    indexSong = random.Next(0, (ListSong.Count));
                }
                else
                {
                    if (indexSong < ListSong.Count - 1 && indexSong >= 0)
                    {
                        indexSong = indexSong + 1;
                    }
                    else
                    {
                        indexSong = 0;
                    }
                }
            }
            LoadSong(ListSong[indexSong]);
            PlayerMedia.AutoPlay = true;
            Play_Song();
            arraySong.SelectedIndex = indexSong;
        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            if (isVolume)
            {
                if (volume_song.Value == 0)
                {
                    Debug.WriteLine(lastValueVolume);
                    icon_volume.Glyph = "\uE995";
                    volume_song.Value = 100;
                    isVolume = true;
                }
                else
                {
                    Debug.WriteLine(valueVolume);
                    lastValueVolume = valueVolume;
                    icon_volume.Glyph = "\uE74F";
                    volume_song.Value = 0;
                    isVolume = false;
                }

            }
            else
            {
                Debug.WriteLine(lastValueVolume);
                icon_volume.Glyph = iconVolume;
                volume_song.Value = lastValueVolume;
                isVolume = true;
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PlayerMedia.Stop();
            if(PlayerMedia.Position.TotalSeconds > 5 || btnRepeat.IsChecked == null)
            {
                indexSong = indexSong;
            } else
            {
                if (btnShuffle.IsChecked == true)
                {
                    Random random = new Random();
                    indexSong = random.Next(0, (ListSong.Count));
                }
                else
                {
                    if (indexSong > 0)
                    {
                        indexSong = indexSong - 1;
                    }
                    else
                    {
                        indexSong = 0;
                    }
                }
            }
            LoadSong(ListSong[indexSong]);
            PlayerMedia.AutoPlay = true;
            Play_Song();
            arraySong.SelectedIndex = indexSong;
        }
        private void TickTock(object sender, object e)
        {
            if (PlayerMedia.Position.Seconds < 10)
            {
                minTime.Text = PlayerMedia.Position.Minutes + ":0" + PlayerMedia.Position.Seconds;
            }
            else
            {
                minTime.Text = PlayerMedia.Position.Minutes + ":" + PlayerMedia.Position.Seconds;
            }
            if (PlayerMedia.NaturalDuration.TimeSpan.Seconds < 10)
            {
                maxTime.Text = PlayerMedia.NaturalDuration.TimeSpan.Minutes + ":0" + PlayerMedia.NaturalDuration.TimeSpan.Seconds;
            }
            else
            {
                maxTime.Text = PlayerMedia.NaturalDuration.TimeSpan.Minutes + ":" + PlayerMedia.NaturalDuration.TimeSpan.Seconds;
            }
            time_play.Minimum = 0;
            time_play.Maximum = PlayerMedia.NaturalDuration.TimeSpan.TotalSeconds;
            time_play.Value = PlayerMedia.Position.TotalSeconds;
            //Debug.WriteLine(PlayerMedia.Position.TotalSeconds);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (PlayerMedia.Source != null && PlayerMedia.NaturalDuration.HasTimeSpan)
            {
                time_play.Minimum = 0;
                time_play.Maximum = PlayerMedia.NaturalDuration.TimeSpan.TotalSeconds;
                time_play.Value = PlayerMedia.Position.TotalSeconds;
            }
        }

        private void time_play_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double SliderValue = time_play.Value;
            TimeSpan ts = TimeSpan.FromSeconds(SliderValue);
            PlayerMedia.Position = ts;
            //time_play.Value = PlayerMedia.Position.TotalSeconds;
        }

        private void btnShuffle_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            if(btnRepeat.IsChecked == null)
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatOne);
            } else if (btnRepeat.IsChecked == false)
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatAll);
            } else
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatAll);
            }
            Debug.WriteLine(btnRepeat.IsChecked);
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
        }

        private void Do_Reset(object sender, RoutedEventArgs e)
        {
            song_name.Text = song_description.Text = song_singer.Text = song_author.Text = song_thumbnail.Text = song_link.Text = "";
        }

        private void PlayerMedia_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (PlayerMedia.CurrentState.ToString() == "Opening")
            {
                ProgressSong.Visibility = Visibility.Visible;
                TimelineSong.Visibility = Visibility.Collapsed;
            } else
            {
                ProgressSong.Visibility = Visibility.Collapsed;
                TimelineSong.Visibility = Visibility.Visible;
            }
            if (PlayerMedia.CurrentState.ToString() == "Buffering")
            {
                BufferingSong.Visibility = Visibility.Visible;
            } else
            {
                BufferingSong.Visibility = Visibility.Collapsed;
            }
            if (PlayerMedia.CurrentState.ToString() == "Closed")
            {
                if (arraySong.SelectedIndex == ListSong.Count)
                {
                    //Pause_Song();
                }
                else
                {
                    AutoNext();
                }
            }
            //Debug.WriteLine(PlayerMedia.CurrentState);
        }


        private void PlayerMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(arraySong.SelectedIndex +" = "+ ListSong.Count);
            if(btnRepeat.IsChecked == false)
            {
                if (arraySong.SelectedIndex == (ListSong.Count - 1) )
                {
                    AutoNext();
                    PlayerMedia.AutoPlay = false;
                    btnPlay.Icon = new SymbolIcon(Symbol.Play);
                    isPlaying = false;
                }
                else
                {
                    AutoNext();
                }
            } else if (btnRepeat.IsChecked == true)
            {
                AutoNext();
            } else if (btnRepeat.IsChecked == null)
            {
                Pause_Song();
                LoadSong(ListSong[indexSong]);
                Play_Song();
                arraySong.SelectedIndex = indexSong;
            }
            //Debug.WriteLine(btnRepeat.IsChecked + " : " + PlayerMedia.IsLooping);
        }

        private async void LoadAllSongLocal()
        {
            QueryOptions queryOption = new QueryOptions
            (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" });

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            Debug.WriteLine(files[0].Name);
            var fileone = files[0];
            var bitmap = new BitmapImage();
            var stream = await files[0].OpenReadAsync();
            var path = files[0].Path;
            Stream streamer = stream.AsStreamForRead();
            //await bitmap.SetSourceAsync(stream);
            Debug.WriteLine(path);
            //PlayerLocal.Source = new Uri(path);

            //foreach (var file in files)
            //{
            //    Debug.WriteLine(file.Name);
            //}
        }
    }
}
