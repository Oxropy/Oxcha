using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitchat.Logic;

namespace Twitchat.GUI.ViewModel
{
    public class UserMergeViewModel : BaseViewModel
    {
        string standardEmote = "Emote";

        private string insertString;
        public string InsertString
        {
            get { return insertString; }
            set { insertString = value; }
        }

        enum GreetType
        {
            All,
            Friends
        }

        private ObservableCollection<string> userList = new ObservableCollection<string>();
        public ObservableCollection<string> UserList
        {
            get { return userList; }
            set
            {
                userList = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> FriendList = UserAdmin.FriendList;

        private ObservableCollection<string> greetedList = new ObservableCollection<string>();
        public ObservableCollection<string> GreetedList
        {
            get { return greetedList; }
            set
            {
                greetedList = value;
                NotifyPropertyChanged();
            }
        }

        public UserMergeViewModel(ICollection<string> userList)
        {
            foreach (var user in userList)
            {
                UserList.Add(user);
            }
        }

        public ICollection<string> GetMergedList()
        {
            //TODO: einstellung ob alle oder nur freunde
            GreetType gt = GreetType.All;

            IEnumerable<string> greetUserList = new List<string>();
            switch (gt)
            {
                case GreetType.All:
                    // Alle aus dem Chat, die noch nicht gegrüßt wurden
                    greetUserList = UserList.Where(u => !greetedList.Contains(u)).OrderBy(u => u);

                    break;
                case GreetType.Friends:
                    // Nur Freunde aus dem Chat, die noch nicht gegrüßt wurden
                    greetUserList = UserList.Where(u => FriendList.Contains(u) && !greetedList.Contains(u));

                    break;
            }

            // Emote davor setzen
            //TODO: bei Freunden gesondertes Emote verwenden, wenn vorhanden
            ICollection<string> greetList = new List<string>();
            foreach (string user in greetUserList)
            {
                string emote = standardEmote;

                if (FriendList.Contains(user))
                {
                    string friendEmote = UserAdmin.GetFriendEmote(user);
                    if (!string.IsNullOrWhiteSpace(friendEmote))
                    {
                        emote = friendEmote;
                    }
                }
                greetList.Add(string.Format("{0} {1}", emote, user));
            }

            ICollection<string> result = StringUtils.MergeByTrimSize(greetList, 500);

            // Gegrüßte der Gegrüßtenliste hinzufügen
            greetUserList.ToList().ForEach(g => GreetedList.Add(g));

            return result;
        }
    }
}
