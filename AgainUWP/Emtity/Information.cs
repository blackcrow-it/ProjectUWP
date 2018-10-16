using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgainUWP.Emtity
{
    class Information
    {
        public long _id;
        public string _firstName;
        public string _lastName;
        public string _avatar;
        public string _phone;
        public string _address;
        public string _introduction;
        public short _gender;
        public string _birthday;
        public string _email;
        public string _password;
        public string _salt;
        public string _createdAt;
        public string _updatedAt;
        public short _status;

        public long id { get => _id; set => _id = value; }
        public string firstName { get => _firstName; set => _firstName = value; }
        public string lastName { get => _lastName; set => _lastName = value; }
        public string avatar { get => _avatar; set => _avatar = value; }
        public string phone { get => _phone; set => _phone = value; }
        public string address { get => _address; set => _address = value; }
        public string introduction { get => _introduction; set => _introduction = value; }
        public short gender { get => _gender; set => _gender = value; }
        public string birthday { get => _birthday; set => _birthday = value; }
        public string email { get => _email; set => _email = value; }
        public string password { get => _password; set => _password = value; }
        public string salt { get => _salt; set => _salt = value; }
        public string createdAt { get => _createdAt; set => _createdAt = value; }
        public string updatedAt { get => _updatedAt; set => _updatedAt = value; }
        public short status { get => _status; set => _status = value; }
    }
}
