using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    public interface ICChat
    {

        void Receive(string nick, string msg);
        void ReceiveNewConnection(string nick, bool f);
    }
    public interface IServer
    {
        bool AddUser(string nick, string URL);
        void WriteInChat(string nick, string msg);

        void RemoveUser(string nick, string URL);
    }
}
