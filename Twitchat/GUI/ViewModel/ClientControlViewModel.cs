using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Twitchat.Logic;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using System.Collections.Generic;

namespace Twitchat.GUI.ViewModel
{
    public class ClientControlViewModel : BaseViewModel
    {
        #region Eigenschaften und Felder
        TwitchClient client;
        ConnectionCredentials credentials;

        private string inputText  = string.Empty;
        public string InputText
        {
            get { return inputText; }
            set { inputText = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility chatsVisibility = Visibility.Visible;
        public Visibility ChatsVisibility
        {
            get { return chatsVisibility; }
            set { chatsVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ClientControlListItemViewModel> clientChats = new ObservableCollection<ClientControlListItemViewModel>();
        public ObservableCollection<ClientControlListItemViewModel> ClientChats
        {
            get { return clientChats; }
            set
            {
                clientChats = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<string, ChatControlViewModel> chatControlList = new Dictionary<string, ChatControlViewModel>();
        public Dictionary<string, ChatControlViewModel> ChatControlList
        {
            get { return chatControlList; }
            set
            {
                chatControlList = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Konstruktor
        public ClientControlViewModel(string user, string accesToken)
        {
            credentials = new ConnectionCredentials(user, accesToken);
            client = new TwitchClient(credentials);

            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;

            client.Connect();

            // Channel muss gejoint sein um in der liste aufgeführt zu werden
            //TODO: Chats speichern und aus der DB laden
            JoinChat("oxropy");
        }
        #endregion

        #region Methoden
        public void AddChannel()
        {
            JoinChat(inputText);
            InputText = string.Empty;
        }

        private void JoinChat(string channel)
        {
            client.JoinChannel(channel);
        }
        #endregion

        #region Events
        private void Client_OnJoinedChannel(object sender, TwitchLib.Events.Client.OnJoinedChannelArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ClientChats.Add(new ClientControlListItemViewModel(e.Channel))));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList.Add(e.Channel, new ChatControlViewModel(client, e.Channel))));
        }
        
        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[e.Channel].ChatterList.Remove(new ViewerListItemViewModel(e.Username))));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[e.Channel].ChatterList.Remove(e.Username)));
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[e.Channel].ChatterList.Add(new ViewerListItemViewModel(e.Username))));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[e.Channel].ChatterList.Add(e.Username)));
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            ChatMessage message = e.ChatMessage;
            string mess = string.Format("{0}: {1}", message.Username, message.Message);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[message.Channel].ChatList.Add(mess)));
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ChatControlList[e.BotUsername].ChatList.Add(string.Format("Connected to {0}", e.BotUsername))));
        }
        #endregion
    }
}
