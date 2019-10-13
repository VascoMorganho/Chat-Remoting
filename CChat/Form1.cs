using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using SharedInterfaces;

namespace CChat
{
    public partial class Form1 : Form
    {

        IServer s = null;

        public static Form1 Form1Instance;
        public string nickn;
        public string cport;
        public string turl;
        int d = 0;
        public Form1()
        {         
            InitializeComponent();
            Form1Instance = this;
        }

        public void Connect(string nick, string port)
        {
            this.d++;
            SetText($"Connecting...\r\n");
            this.nickn = nick;
            this.cport = port;
            TcpChannel c = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(c, false);

            CChat cc = new CChat(nick);

            RemotingServices.Marshal(cc, "ClientChat"+this.d, typeof(CChat));

            this.s = (IServer)Activator.GetObject(typeof(IServer), "tcp://192.168.1.100:8086/SuperChat");
            this.turl = "tcp://"+ GetLocalIPAddress() + ":" + port + "/ClientChat" + this.d;

            if(s.AddUser(nick, turl) == false)  //Check for already taken nickname
            {
                SetText($"Nickname already taken. Try again.\r\n");
                c.StopListening(null);
                RemotingServices.Disconnect(this);
                ChannelServices.UnregisterChannel(c);
                c = null;
            }
            else { 
                SetText($"Connected!\r\n");
                portButton.Enabled = false;
            }
            
        }

        public void Write(string msg)
        {
            s.WriteInChat(this.nickn, msg);
            chatTextBox.AppendText($"{this.nickn} : {msg}\r\n");
        }

        public void WriteNewCon(string nick, bool f)
        {
            if (f == true) { SetText($"{nick} has joined the chat.\r\n"); }
            else { SetText($"{nick} has left the chat.\r\n"); }
        }

        public void WriteFinal(string nick, string msg)
        {
            SetText($"{nick} : {msg}\r\n");
        }

        private void portButton_Click(object sender, EventArgs e)
        {
            Connect(nickTextBox.Text, portTextBox.Text);
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            Write(sendTextBox.Text);
            sendTextBox.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) // Notify Server that user has left
        {
            base.OnFormClosing(e);
            if(s != null)
                s.RemoveUser(this.nickn, this.turl);
        }

        delegate void SetTextCallback(string text);

        private void SetText(string text)  //Tive de usar este metodo porque estava a ter erro de threads
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.chatTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.chatTextBox.Text += text;
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
