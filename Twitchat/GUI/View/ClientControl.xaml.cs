using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Twitchat.GUI.ViewModel;

namespace Twitchat.GUI.View
{
    /// <summary>
    /// Interaktionslogik für ClientControl.xaml
    /// </summary>
    public partial class ClientControl : UserControl
    {
        public ClientControl()
        {
            InitializeComponent();
        }

        private void AddChat_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ClientControlViewModel vm)
            {
                vm.AddChannel();
            }
        }

        private void ChatsVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ClientControlViewModel vm)
            {
                if (vm.ChatsVisibility == Visibility.Collapsed)
                {
                    vm.ChatsVisibility = Visibility.Visible;
                }
                else
                {
                    vm.ChatsVisibility = Visibility.Collapsed;
                }
            }
        }
    }
}
