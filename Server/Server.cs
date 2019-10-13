using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using SharedInterfaces;

namespace Server
{
    class Server : MarshalByRefObject, IServer
    {

        public Server() { }

        List<User> users = new List<User>();
        public bool AddUser(string nick, string URL) { //Add a new user that just connected to the new list of users

            foreach (var user in users) 
            {
                if(user.Nick == nick)
                { 
                    return false; 
                }
            }

            ICChat c = (ICChat)Activator.GetObject(typeof(ICChat), URL);
            users.Add(new User(nick, URL, c));
            System.Console.WriteLine($"User {nick} joined the chat.");

            foreach (var user in users.Where(x => x.Nick != nick)) //Notify other users that someone joinned
            {
                user.getICChat().ReceiveNewConnection(nick, true); ;
            }
            return true;
        }
        public void WriteInChat(string nick, string msg)  //Sends a new message to all useres except the one who sent it
        {
            Console.WriteLine($"Message \"{msg}\" received by \"{nick}\"");

            foreach (var user in users.Where(x => x.Nick != nick)) { 
                System.Console.WriteLine($"Message ({msg}) beeing sent to {user.Url} .");
                user.getICChat().Receive(nick, msg); ;

                System.Console.WriteLine($"Message ({msg}) sent to {user.Url} .");
            }
        }

        public void RemoveUser(string nick, string URL) //Remove a user from the list of users
        {
            User userToRemove = users.Single(x => x.Nick == nick && x.Url == URL);
            users.Remove(userToRemove);
            System.Console.WriteLine($"User {nick} left the chat.");

            foreach (var user in users.Where(x => x.Nick != nick)) //Notify other users that someone left
            {
                user.getICChat().ReceiveNewConnection(nick, false); ;
            }

        }

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            ChannelDataStore data = (ChannelDataStore)channel.ChannelData;
            foreach (string uri in data.ChannelUris)
            {
                Console.WriteLine("The channel URI is {0}.", uri);
            }
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Server), "SuperChat", WellKnownObjectMode.Singleton);
            string[] urls = channel.GetUrlsForUri("SuperChat");
            if (urls.Length > 0)
            {
                string objectUrl = urls[0];
                string objectUri;
                string channelUri = channel.Parse(objectUrl, out objectUri);
                Console.WriteLine("The object URL is {0}.", objectUrl);
                Console.WriteLine("The object URI is {0}.", objectUri);
                Console.WriteLine("The channel URI is {0}.", channelUri);
            }
            System.Console.WriteLine("<enter> to leave...");
            System.Console.ReadLine();

        }
    }
}
