using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitchat.Logic.DataContract;

namespace Twitchat.Logic
{
    static class UserAdmin
    {
        public static ObservableCollection<string> FriendList = new ObservableCollection<string>();

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

        public static string GetFriendEmote(string userName)
        {
            return MainAdmin.GetFriendEmote(userName);
        }
    }
}
