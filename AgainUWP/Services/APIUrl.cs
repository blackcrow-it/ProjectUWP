using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgainUWP.Services
{
    class APIUrl
    {
        public static string GET_UPLOAD_TOKEN = "https://2-dot-backup-server-002.appspot.com/get-upload-token";
        public static string API_REGISTER = "https://2-dot-backup-server-002.appspot.com/_api/v2/members";
        public static string API_LOGIN = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
        public static string API_INFORMATION = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/information";
        public static string API_SONG = "https://2-dot-backup-server-002.appspot.com/_api/v2/songs";
        public static string API_MY_SONG = "https://2-dot-backup-server-002.appspot.com/_api/v2/songs/get-mine";
    }
}
