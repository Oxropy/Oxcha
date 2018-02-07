using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.GUI.ViewModel
{
    public class ClientControlListItemViewModel : BaseViewModel
    {
        // Name des Channels
        private string channelName;
        public string ChannelName
        {
            get { return channelName; }
            set { channelName = value;
                NotifyPropertyChanged();
            }
        }

        // Art des Chats (Stream, Whisper, Privat)
        private string chatType;
        public string ChatType
        {
            get { return chatType; }
            set { chatType = value;
                NotifyPropertyChanged();
            }
        }

        // Anzahl der Zuschauer
        private string viewerCount;
        public string ViewerCount
        {
            get { return viewerCount; }
            set { viewerCount = value;
                NotifyPropertyChanged();
            }
        }

        // Anzahl der im Chat befindenen User
        private string chatterCount;
        public string ChatterCount
        {
            get { return chatterCount; }
            set { chatterCount = value;
                NotifyPropertyChanged();
            }
        }

        // Status ob Streamer live ist
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value;
                NotifyPropertyChanged();
            }
        }
        
        public ClientControlListItemViewModel(string channel)
        {
            ChannelName = channel;
            ChatType = "Channel"; // Whisper

            ViewerCount = 5.ToString();
            ChatterCount = 3.ToString();
        }
    }
}
