using System.Text;
using System.Windows;
using Twitchat.Logic;
using Twitchat.Resources;
using Twitchat.GUI.ViewModel;

namespace Twitchat.GUI.View
{
    /// <summary>
    /// Interaktionslogik für TwitchAuthenticationWindow.xaml
    /// </summary>
    public partial class TwitchAuthenticationWindow : Window
    {
        public string Url = null;

        public TwitchAuthenticationWindow(string url)
        {
            InitializeComponent();

            webBrowser.Navigate(url);
        }

        private void webBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Uri.Host == TwitchResource.RedirectHost)
            {
                Url = e.Uri.Query;
            }
            
            var windows = Application.Current.Windows;
            windows[1].Close();
        }
    }
}
