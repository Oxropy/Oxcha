using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Models.Client;

namespace Twitchat.Twitch
{
    public class Channel
    {
        public Client Client { get; private set; }
        public string Name { get; private set; }
        public IChannelHandler Handler { get; private set; }

        public Channel(Client client, string channel, IChannelHandler handler)
        {
            this.Client = client;
            this.Name = channel;
            this.Handler = handler;
        }

        public void LeaveChannel()
        {
            Client.LeaveChannel(Name);
        }

        public void SendMessage(string message)
        {
            Client.SendMessage(Name, message);
        }
    }
}
