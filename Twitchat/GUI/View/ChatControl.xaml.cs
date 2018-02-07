using System.Windows.Controls;
using System.Windows.Input;
using Twitchat.GUI.ViewModel;
using TwitchLib;

namespace Twitchat.GUI.View
{
    /// <summary>
    /// Interaktionslogik für ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        public ChatControl()
        {
            InitializeComponent();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ChatControlViewModel vm = (ChatControlViewModel)this.DataContext;
                vm.SendMessage();
            }
        }
    }
}
