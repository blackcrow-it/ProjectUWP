using AgainUWP.Dialog;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AgainUWP.Data;
using Microsoft.Data.Sqlite;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

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
        private int OnTab = 1;
        private bool isInternetConnected = NetworkInterface.GetIsNetworkAvailable();
        TimeSpan _position;
        Information information = new Information();
        DispatcherTimer _timer = new DispatcherTimer();
        private ObservableCollection<Song> listSong;
        private ObservableCollection<Song> listSongRecent;
        private ObservableCollection<SongLocal> listSongLocal;
        public ObservableCollection<SongLocal> ListSongLocal { get => listSongLocal; set => listSongLocal = value; }
        public ObservableCollection<Song> ListSongRecent { get => listSongRecent; set => listSongRecent = value; }
        public ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }

        public ListViewDemo()
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            bool isWLANConnection = (InternetConnectionProfile == null) ? false : InternetConnectionProfile.IsWlanConnectionProfile;
            this.ListSong = new ObservableCollection<Song>();
            this.ListSongRecent = new ObservableCollection<Song>();
            this.ListSongLocal = new ObservableCollection<SongLocal>();
            this.InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += TickTock;
            _timer.Start();
            LoadSongInMusicLibrary();
            if (isInternetConnected || isWLANConnection)
            {
                LoadAllSongRecent(ListSongRecent);
                LoadAllSong(ListSong);
                GetAvatarHeader();
                IsLogIn();
            }
            else
            {

                pivot.SelectedIndex = 2;
            }
        }
        private void LoadToPlaySong(ObservableCollection<Song> ListSongs, int index, ListView listView)
        {
            Pause_Song();
            LoadSong(ListSongs[index]);
            time_play.Value = 0;
            PlayerMedia.AutoPlay = true;
            Play_Song();
            listView.SelectedIndex = indexSong;
        }
        private void LoadToPlaySongLocal(ObservableCollection<SongLocal> ListSongsLocal, int index, GridView listView)
        {
            Pause_Song();
            LoadSongLocal(ListSongsLocal[index]);
            time_play.Value = 0;
            PlayerMedia.AutoPlay = true;
            Play_Song();
            listView.SelectedIndex = index;
        }

        private async static void LoadAllSong(ObservableCollection<Song> listSong)
        {
            listSong.Clear();
            string token;
            try
            {
                token = await APIHandle.GetToken();
                var httpResponseMessage = APIHandle.GetData(APIUrl.API_SONG, "Basic", token);
                var informationJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                var SongModel = JsonConvert.DeserializeObject<ObservableCollection<Song>>(informationJson);

                foreach (var data in SongModel)
                {
                    listSong.Insert(0, data);
                }
                for(int i = listSong.Count - 1; i == 10; i--)
                {
                    //listSong.RemoveAt(i);
                    
                }
                Debug.WriteLine("i");
            }
            catch
            {
                Debug.WriteLine("Error");
            }
        }
        private static void LoadAllSongRecent(ObservableCollection<Song> listSongRecent)
        {
            listSongRecent.Clear();
            using (SqliteConnection db =
                new SqliteConnection("Filename=Song.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT name, description, singer, author, thumbnail, link FROM Recent ORDER BY updatedAt DESC LIMIT 5;", db);
                SqliteDataReader query = selectCommand.ExecuteReader();
                while (query.Read())
                {
                    listSongRecent.Add(new Song
                    {
                        name = query.GetString(0),
                        description = query.GetString(1),
                        singer = query.GetString(2),
                        author = query.GetString(3),
                        thumbnail = query.GetString(4),
                        link = query.GetString(5),
                    });
                }
                db.Close();
            }
            Debug.WriteLine("Ok");
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            Song song = grid.Tag as Song;
            indexSong = arraySong.SelectedIndex;
            LoadSong(song);
            DataHandle.AddRecentSong(song);
            LoadAllSongRecent(ListSongRecent);
            PlayerMedia.AutoPlay = true;
            Play_Song();
            OnTab = 1;
        }
        private void Grid_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            Song song = grid.Tag as Song;
            indexSong = arraySongRecent.SelectedIndex;
            LoadSong(song);
            PlayerMedia.AutoPlay = true;
            Play_Song();
            OnTab = 2;
        }

        private void LoadSong(Emtity.Song currentSong)
        {
            try
            {
                PlayerMedia.Source = new Uri(currentSong.link);
                PlayerMedia.AutoPlay = false;
            }
            catch
            {
                PlayerMedia.Source = new Uri("https://mp3.zing.vn/bai-hat/Nen-Va-Hoa-Touliver-Remix-Touliver-Rhymastic/ZW9D89D8.html?play_song=1.mp3");
            }
            this.name_song.Text = currentSong.name;
            this.singer_song.Text = currentSong.singer;
            //this.avatar_song. = currentSong.singer;
            try
            {
                this.avatar_song.ImageSource = new BitmapImage(new Uri(currentSong.thumbnail, UriKind.Absolute));
            }
            catch
            {
                Debug.WriteLine("error uri thumnail");
                this.avatar_song.ImageSource = new BitmapImage(new Uri("https://cdn1.iconfinder.com/data/icons/music-12/96/CD-2-512.png", UriKind.Absolute));
            }
        }
        private void LoadSongLocal(Emtity.SongLocal currentSong)
        {
            time_play.Value = 0;
            PlayerMedia.SetSource(currentSong.stream, currentSong.type);
            PlayerMedia.AutoPlay = true;
            this.name_song.Text = currentSong.name;
            this.singer_song.Text = currentSong.singer;
            //this.avatar_song.DisplayName = currentSong.singer;
            this.avatar_song.ImageSource = currentSong.thumnail;
        }
        private async void LoadSongFromFolder(IReadOnlyList<StorageFile> files)
        {
            if (files.Count > 0)
            {
                foreach (StorageFile file in files)
                {
                    MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    //Debug.WriteLine(file.Path);
                    const uint requestedSize = 190;
                    const ThumbnailMode thumbnailMode = ThumbnailMode.MusicView;
                    const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
                    var thumbnailAvatar = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions);
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(thumbnailAvatar);
                        ListSongLocal.Add(new SongLocal()
                        {
                            name = musicProperties.Title,
                            singer = musicProperties.Artist,
                            author = musicProperties.AlbumArtist,
                            album = musicProperties.Album,
                            time = musicProperties.Duration.ToString().Split('.')[0],
                            thumnail = bitmapImage,
                            stream = await file.OpenAsync(FileAccessMode.Read),
                            type = file.ContentType
                        });
                    }
                    );
                }
            }
        }
        private async void LoadSongInMusicLibrary()
        {
            QueryOptions queryOption = new QueryOptions
        (CommonFileQuery.OrderByTitle, new string[] { ".mp3" });

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            ListSongLocal.Clear();
            LoadSongFromFolder(files);
        }

        private async void ChooseFolderLocal_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".mp3");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                ListSongLocal.Clear();
                LinkFolderLocal.Text = folder.Path;
                //get the file list
                var files = await folder.GetFilesAsync(CommonFileQuery.OrderByName).AsTask().ConfigureAwait(false);

                //For example, I can use the following code to play the first item
                LoadSongFromFolder(files);
            }
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            //Debug.WriteLine(arraySongLocal.SelectedIndex);
            Grid grid = sender as Grid;
            SongLocal selectedSong = grid.Tag as SongLocal;
            indexSong = arraySongLocal.SelectedIndex;
            LoadSongLocal(selectedSong);
            Play_Song();
            OnTab = 3;
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
        private void AutoNextSong(ObservableCollection<Song> ListSongs, ListView listView)
        {
            if (btnShuffle.IsChecked == true)
            {
                Random random = new Random();
                indexSong = random.Next(0, (ListSongs.Count));
            }
            else
            {
                if (indexSong < ListSongs.Count - 1 && indexSong >= 0)
                {
                    indexSong = indexSong + 1;
                }
                else
                {
                    indexSong = 0;
                }
            }
            LoadToPlaySong(ListSongs, indexSong, listView);
        }
        private void AutoNextSongLocal(ObservableCollection<SongLocal> ListSongsLocal, GridView listViewLocal)
        {
            if (btnShuffle.IsChecked == true)
            {
                Random random = new Random();
                indexSong = random.Next(0, (ListSongsLocal.Count));
            }
            else
            {
                if (indexSong < ListSongsLocal.Count - 1 && indexSong >= 0)
                {
                    indexSong = indexSong + 1;
                }
                else
                {
                    indexSong = 0;
                }
            }
            if (indexSong == 0)
            {
                time_play.Value = 0;
            }
            LoadToPlaySongLocal(ListSongsLocal, indexSong, listViewLocal);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            PlayerMedia.Stop();
            switch (OnTab)
            {
                case 1:
                    if (btnRepeat.IsChecked == null)
                    {
                        indexSong = indexSong;
                        LoadToPlaySong(ListSong, indexSong, arraySong);
                        DataHandle.AddRecentSong(ListSong[indexSong]);
                        LoadAllSongRecent(ListSongRecent);
                    }
                    else
                    {
                        AutoNextSong(ListSong, arraySong);
                        DataHandle.AddRecentSong(ListSong[indexSong]);
                        LoadAllSongRecent(ListSongRecent);
                    }
                    break;
                case 2:
                    if (btnRepeat.IsChecked == null)
                    {
                        indexSong = indexSong;
                        LoadToPlaySong(ListSongRecent, indexSong, arraySongRecent);
                    }
                    else
                    {
                        AutoNextSong(ListSongRecent, arraySongRecent);
                    }
                    break;
                case 3:
                    if (btnRepeat.IsChecked == null)
                    {
                        indexSong = indexSong;
                        LoadToPlaySongLocal(ListSongLocal, indexSong, arraySongLocal);
                    }
                    else
                    {
                        AutoNextSongLocal(ListSongLocal, arraySongLocal);
                    }
                    break;
                default:
                    break;
            }
        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {
            if (isVolume)
            {
                if (volume_song.Value == 0)
                {
                    icon_volume.Glyph = "\uE995";
                    volume_song.Value = 100;
                    isVolume = true;
                }
                else
                {
                    lastValueVolume = valueVolume;
                    icon_volume.Glyph = "\uE74F";
                    volume_song.Value = 0;
                    isVolume = false;
                }

            }
            else
            {
                icon_volume.Glyph = iconVolume;
                volume_song.Value = lastValueVolume;
                isVolume = true;
            }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            PlayerMedia.Stop();
            switch (OnTab)
            {
                case 1:
                    if (PlayerMedia.Position.TotalSeconds > 5 || btnRepeat.IsChecked == null)
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
                    LoadToPlaySong(ListSong, indexSong, arraySong);
                    DataHandle.AddRecentSong(ListSong[indexSong]);
                    LoadAllSongRecent(ListSongRecent);
                    break;
                case 2:
                    if (PlayerMedia.Position.TotalSeconds > 5 || btnRepeat.IsChecked == null)
                    {
                        indexSong = indexSong;
                    }
                    else
                    {
                        if (btnShuffle.IsChecked == true)
                        {
                            Random random = new Random();
                            indexSong = random.Next(0, (ListSongRecent.Count));
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
                    LoadToPlaySong(ListSongRecent, indexSong, arraySongRecent);
                    DataHandle.AddRecentSong(ListSongRecent[indexSong]);
                    LoadAllSongRecent(ListSongRecent);
                    break;
                case 3:
                    if (PlayerMedia.Position.TotalSeconds > 5 || btnRepeat.IsChecked == null)
                    {
                        indexSong = indexSong;
                    }
                    else
                    {
                        if (btnShuffle.IsChecked == true)
                        {
                            Random random = new Random();
                            indexSong = random.Next(0, (ListSongLocal.Count));
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
                    LoadToPlaySongLocal(ListSongLocal, indexSong, arraySongLocal);
                    break;
                default:
                    break;
            }
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
        }

        private void btnRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (btnRepeat.IsChecked == null)
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatOne);
            }
            else if (btnRepeat.IsChecked == false)
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatAll);
            }
            else
            {
                btnRepeat.Icon = new SymbolIcon(Symbol.RepeatAll);
            }
        }
        private void PlayerMedia_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (PlayerMedia.CurrentState.ToString() == "Opening")
            {
                ProgressSong.Visibility = Visibility.Visible;
                TimelineSong.Visibility = Visibility.Collapsed;
            }
            else
            {
                ProgressSong.Visibility = Visibility.Collapsed;
                TimelineSong.Visibility = Visibility.Visible;
            }
            if (PlayerMedia.CurrentState.ToString() == "Buffering")
            {
                BufferingSong.Visibility = Visibility.Visible;
            }
            else
            {
                BufferingSong.Visibility = Visibility.Collapsed;
            }
            if (PlayerMedia.CurrentState.ToString() == "Closed")
            {
                switch (OnTab)
                {
                    case 1:
                        if (arraySong.SelectedIndex == ListSong.Count)
                        {
                            //Pause_Song();
                        }
                        else
                        {
                            AutoNextSong(ListSong, arraySong);
                            DataHandle.AddRecentSong(ListSong[indexSong]);
                            LoadAllSongRecent(ListSongRecent);
                        }
                        break;
                    case 2:
                        if (arraySongRecent.SelectedIndex == ListSongRecent.Count)
                        {
                            //Pause_Song();
                        }
                        else
                        {
                            AutoNextSong(ListSongRecent, arraySongRecent);
                            Play_Song();
                        }
                        break;
                    case 3:
                        if (arraySongLocal.SelectedIndex == ListSongLocal.Count)
                        {
                            //Pause_Song();
                        }
                        else
                        {
                            AutoNextSongLocal(ListSongLocal, arraySongLocal);
                        }
                        break;
                    default:
                        break;
                }

            }
        }


        private void PlayerMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (OnTab)
            {
                case 1:
                    if (btnRepeat.IsChecked == false)
                    {
                        if (arraySong.SelectedIndex == (ListSong.Count - 1))
                        {
                            AutoNextSong(ListSong, arraySong);
                            PlayerMedia.AutoPlay = false;
                            btnPlay.Icon = new SymbolIcon(Symbol.Play);
                            isPlaying = false;
                        }
                        else
                        {
                            AutoNextSong(ListSong, arraySong);
                            DataHandle.AddRecentSong(ListSong[indexSong]);
                            LoadAllSongRecent(ListSongRecent);
                        }
                    }
                    else if (btnRepeat.IsChecked == true)
                    {
                        AutoNextSong(ListSong, arraySong);
                        DataHandle.AddRecentSong(ListSong[indexSong]);
                        LoadAllSongRecent(ListSongRecent);
                    }
                    else if (btnRepeat.IsChecked == null)
                    {
                        LoadToPlaySong(ListSong, indexSong, arraySong);
                        DataHandle.AddRecentSong(ListSong[indexSong]);
                        LoadAllSongRecent(ListSongRecent);
                    }
                    break;
                case 2:
                    if (btnRepeat.IsChecked == false)
                    {
                        if (arraySongRecent.SelectedIndex == (ListSongLocal.Count - 1))
                        {
                            AutoNextSong(ListSongRecent, arraySongRecent);
                            PlayerMedia.AutoPlay = false;
                            btnPlay.Icon = new SymbolIcon(Symbol.Play);
                            isPlaying = false;
                        }
                        else
                        {
                            AutoNextSong(ListSongRecent, arraySongRecent);
                        }
                    }
                    else if (btnRepeat.IsChecked == true)
                    {
                        AutoNextSong(ListSongRecent, arraySongRecent);
                    }
                    else if (btnRepeat.IsChecked == null)
                    {
                        LoadToPlaySong(ListSongRecent, indexSong, arraySongRecent);
                    }
                    break;
                case 3:
                    if (btnRepeat.IsChecked == false)
                    {
                        if (arraySongLocal.SelectedIndex == (ListSongLocal.Count - 1))
                        {
                            AutoNextSongLocal(ListSongLocal, arraySongLocal);
                            PlayerMedia.AutoPlay = false;
                            btnPlay.Icon = new SymbolIcon(Symbol.Play);
                            isPlaying = false;
                        }
                        else
                        {
                            AutoNextSongLocal(ListSongLocal, arraySongLocal);
                        }
                    }
                    else if (btnRepeat.IsChecked == true)
                    {
                        AutoNextSongLocal(ListSongLocal, arraySongLocal);
                    }
                    else if (btnRepeat.IsChecked == null)
                    {
                        LoadToPlaySongLocal(ListSongLocal, indexSong, arraySongLocal);
                    }
                    break;
                default:
                    break;
            }


            //Debug.WriteLine(btnRepeat.IsChecked + " : " + PlayerMedia.IsLooping);
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            RegisterDialog register = new RegisterDialog();
            await register.ShowAsync();
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            LogInDialog login = new LogInDialog();
            await login.ShowAsync();
        }

        private async void Get_Information(object sender, RoutedEventArgs e)
        {
            InformationDialog infor = new InformationDialog();
            await infor.ShowAsync();
        }

        private async void GetAvatarHeader()
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                if (token != "")
                {
                    var httpResponseMessage = APIHandle.GetData(APIUrl.API_INFORMATION, "Basic", token);
                    //Debug.WriteLine(httpResponseMessage.Result.StatusCode);
                    if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                    {
                        var informationJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                        //Debug.WriteLine(informationJson);
                        information = JsonConvert.DeserializeObject<Information>(informationJson);
                        try
                        {
                            this.avtarHeader.ProfilePicture = new BitmapImage(new Uri(information.avatar, UriKind.Absolute));
                        }
                        catch
                        {

                        }

                    }

                }
            }
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Handle.WriteFile("credential.txt", "");
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Views.ListViewDemo));
                Debug.WriteLine("Logout");
            }
            catch
            {
                Debug.WriteLine("Error Logout");
            }
        }
        private async void IsLogIn()
        {
            string text = await APIHandle.CheckCredential();
            if (text != "")
            {
                string token = await APIHandle.GetToken();
                var httpResponseMessage = APIHandle.GetData(APIUrl.API_INFORMATION, "Basic", token);
                //Debug.WriteLine(httpResponseMessage.Result.StatusCode);
                if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                {
                    btnAddSong.Visibility = Visibility.Visible;
                    btnInfor.Visibility = Visibility.Visible;
                    messageError.Visibility = Visibility.Collapsed;
                    arraySong.Visibility = Visibility.Visible;
                    btnSign.Visibility = Visibility.Collapsed;
                }
                else
                {
                    btnAddSong.Visibility = Visibility.Collapsed;
                    btnInfor.Visibility = Visibility.Collapsed;
                    messageError.Visibility = Visibility.Visible;
                    arraySong.Visibility = Visibility.Collapsed;
                    btnSign.Visibility = Visibility.Visible;
                }

            }
            else
            {
                btnAddSong.Visibility = Visibility.Collapsed;
                btnInfor.Visibility = Visibility.Collapsed;
                messageError.Visibility = Visibility.Visible;
                arraySong.Visibility = Visibility.Collapsed;
                btnSign.Visibility = Visibility.Visible;
            }
        }

        private async void AddSong_Click(object sender, RoutedEventArgs e)
        {
            AddSongDialog addsong = new AddSongDialog();
            await addsong.ShowAsync();
        }

        private async void HyperlinkSignIn(object sender, RoutedEventArgs e)
        {
            LogInDialog logInDialog = new LogInDialog();
            await logInDialog.ShowAsync();
        }

        private async void HyperlinkSignUp(object sender, RoutedEventArgs e)
        {
            RegisterDialog registerDialog = new RegisterDialog();
            await registerDialog.ShowAsync();
        }

        private void SaveCustom_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(colorBackground.Color);
            barMusic.Background = new SolidColorBrush(colorBackground.Color);
            Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] = new SolidColorBrush(colorBackground.Color);
            Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"] = new SolidColorBrush(colorBackground.Color);
            //uriBackground.ImageSource = new BitmapImage(new Uri(linkBackground.Text));
            Debug.WriteLine(Application.Current.Resources["SystemControlHighlightListAccentLowBrush"]);
            settingFlyput.Hide();
        }
    }
}
