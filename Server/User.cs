using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;

namespace Server
{
    class User
    {
        public string nick;
        public string url;
        public ICChat ichat;

        public User(string nick, string url, ICChat ic)
        {
            this.Nick = nick;
            this.Url = url;
            this.ichat = ic;
        }

        public ICChat getICChat()
        {
            return this.ichat;
        }
        public String Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public String Url
        {
            get { return url; }
            set { url = value; }
        }

    }
}
