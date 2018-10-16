using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgainUWP.Emtity
{
    class User
    {
        private string _token;
        private string _secretToken;
        private long _userId;
        private long _createdTimeMLS;
        private long _expiredTimeMLS;
        private long _status;

        public string token { get => _token; set => _token = value; }
        public string secretToken { get => _secretToken; set => _secretToken = value; }
        public long userId { get => _userId; set => _userId = value; }
        public long createdTimeMLS { get => _createdTimeMLS; set => _createdTimeMLS = value; }
        public long expiredTimeMLS { get => _expiredTimeMLS; set => _expiredTimeMLS = value; }
        public long status { get => _status; set => _status = value; }
    }
}
