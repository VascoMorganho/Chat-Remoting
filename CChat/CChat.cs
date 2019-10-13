using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using SharedInterfaces;

namespace CChat
{
    class CChat : MarshalByRefObject, ICChat
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public string nick;
        public string url;


        public string Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public CChat(string nick)
        {
            this.Nick = nick;
        }
        public void ReceiveNewConnection(string nick, bool f) 
        {
                Form1.Form1Instance.WriteNewCon(nick, f);
        }


        public void Receive(string nick, string msg) {
            
            Form1.Form1Instance.WriteFinal(nick, msg);          
        }

        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
