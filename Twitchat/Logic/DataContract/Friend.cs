using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.Logic.DataContract
{
    public class Friend : UserInfo
    {
        public string Alias { get; private set; }

        public string Emote { get; private set; }

        public bool Greeted { get; private set; }

        public Friend(string userName, string alias = "", string emote = "") : base(userName)
        {
            Alias = alias;
            Emote = emote;
        }

        public void SetAlias(string alias)
        {
            Alias = alias;
            UpdateFriendAlias(UserName, alias);
        }

        public void SetEmote(string emote)
        {
            Emote = emote;
            UpdateFriendEmote(UserName, emote);
        }

        #region Statische Methoden
        public static bool AddFriend(string userName, string alias = "")
        {
            return MainAdmin.AddFriend(userName, alias);
        }

        public static bool RemoveFriend(string userName)
        {
            return MainAdmin.DeleteFriend(userName);
        }

        public static bool UpdateFriendAlias(string userName, string alias)
        {
            return MainAdmin.UpdateFriendAlias(userName, alias);
        }

        public static bool UpdateFriendEmote(string userName, string emote)
        {
            return MainAdmin.UpdateFriendEmote(userName, emote);
        }

        public static Friend GetFriendInfo(string userName)
        {
            return MainAdmin.GetFriendInfo(userName);
        }
        #endregion
    }
}
