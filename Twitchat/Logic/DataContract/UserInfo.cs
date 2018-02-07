using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.Logic.DataContract
{
    public class UserInfo
    {
        public string UserName { get; private set; }

        public UserInfo(string userName)
        {
            UserName = userName;
        }
    }
}
