using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AgainUWP.Emtity
{
    public class SongLocal
    {
        private string _name;
        private string _singer;
        private string _album;
        private string _author;
        private string _time;
        private BitmapImage _thumnail;
        private IRandomAccessStream _stream;
        private string _type;

        public string name { get => _name; set => _name = value; }
        public string singer { get => _singer; set => _singer = value; }
        public string album { get => _album; set => _album = value; }
        public string author { get => _author; set => _author = value; }
        public string time { get => _time; set => _time = value; }
        public BitmapImage thumnail { get => _thumnail; set => _thumnail = value; }
        public IRandomAccessStream stream { get => _stream; set => _stream = value; }
        public string type { get => _type; set => _type = value; }
    }
}
