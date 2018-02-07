using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitchat.Twitch;

namespace Oxcha
{
    public class CliClientHandler : IClientHandler
    {
        public void OnConnect(Client client)
        {
            Console.WriteLine("Connected as '{0}'",  client.User);
        }

        public void OnDisconnect(Client client)
        {
            Console.WriteLine("Disconnected with '{0}'", client.User);
        }
    }
}
