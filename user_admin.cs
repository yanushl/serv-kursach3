using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serv_kursach3
{
    public class user_admin
    {
        private string login;
        private string password;


        public user_admin(string l, string p)
        {
            login = l;
            password = p;
        }

        public string Get_Login
        {
            get { return login; }
            set { login = value; }
        }
        public string Get_Password
        {
            get { return password; }
            set { password = value; }
        }

        public string NetWork_Send
        {
            get
            {
                return login + '\n' + password + '\n';
            }
        }

        
    }
}
