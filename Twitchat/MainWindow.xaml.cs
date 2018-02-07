using System.ComponentModel;
using System.Text;
using System.Windows;
using Twitchat.Logic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;
using System.Net;
using Twitchat.GUI.View;
using Twitchat.Logic.IO;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using TwitchLib;
using System.Collections.ObjectModel;
using System;
using Twitchat.GUI.ViewModel;

namespace Twitchat
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ClientControlViewModel> clientControlList = new ObservableCollection<ClientControlViewModel>();
        public ObservableCollection<ClientControlViewModel> ClientControlList
        {
            get { return clientControlList; }
            set { clientControlList = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (Environment.GetCommandLineArgs().Length <= 2)
            {
                MessageBox.Show(string.Format("Too few arguments!\nUsage: {0} <client id> <client secret>", Environment.GetCommandLineArgs()[0]));
                Environment.Exit(1);
                return;
            }

            MainAdmin.RetrieveAuthenticationUsers(Environment.GetCommandLineArgs()[1], Environment.GetCommandLineArgs()[2]);

            foreach (var item in MainAdmin.TwitchUserTokenDict)
            {
                ClientControlList.Add(new ClientControlViewModel(item.Key, item.Value));
            }
        }

        #region INotifyPropertyChanged implementation
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
