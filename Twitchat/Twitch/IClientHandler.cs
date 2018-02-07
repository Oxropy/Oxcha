using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.Twitch
{
    public interface IClientHandler
    {
        void OnConnect(Client client);
        void OnDisconnect(Client client);
    }
}
