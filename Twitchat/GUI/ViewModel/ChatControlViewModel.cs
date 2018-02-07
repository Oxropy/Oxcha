using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.API.Undocumented.Chatters;
using TwitchLib.Models.Client;

namespace Twitchat.GUI.ViewModel
{
    public class ChatControlViewModel : BaseViewModel
    {
        #region Eigenschaften und Felder
        private string channel;
        private TwitchClient client;

        private ObservableCollection<string> chatList = new ObservableCollection<string>();
        public ObservableCollection<string> ChatList
        {
            get { return chatList; }
            set
            {
                chatList = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<string> chatterList = new ObservableCollection<string>();
        public ObservableCollection<string> ChatterList
        {
            get { return chatterList; }
            set
            {
                chatterList = value;
                NotifyPropertyChanged();
            }
        }
        
        private string chatInput;
        public string ChatInput
        {
            get { return chatInput; }
            set
            {
                chatInput = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Konstruktor
        public ChatControlViewModel(TwitchClient client, string channel)
        {
            this.channel = channel;
            this.client = client;
        }
        #endregion

        #region Methoden
        public int GetChatterCount()
        {
            return chatterList.Count;
        }

        public void SendMessage()
        {
            client.SendMessage(channel, chatInput);
            ChatInput = string.Empty;
        }
        #endregion

        #region Events
        
        #endregion
    }
}
