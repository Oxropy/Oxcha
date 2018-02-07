using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitchat.GUI.ViewModel
{
    public class ViewerListItemViewModel : BaseViewModel
    {
        private bool isFirend;
        public bool IsFriend
        {
            get { return isFirend; }
            set { isFirend = value;
                NotifyPropertyChanged();
            }
        }

        private bool isOwner;
        public bool IsOwner
        {
            get { return isOwner; }
            set { isOwner = value;
                NotifyPropertyChanged();
            }
        }


        private bool isMod;
        public bool IsMod
        {
            get { return isMod; }
            set { isMod = value;
                NotifyPropertyChanged();
            }
        }

        private bool isSub;
        public bool IsSub
        {
            get { return isSub; }
            set { isSub = value;
                NotifyPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value;
                NotifyPropertyChanged();
            }
        }

        private string alias;
        public string Alias
        {
            get { return alias; }
            set { alias = value; }
        }


        public ViewerListItemViewModel(string viewer)
        {
            // TODO: Statis von Twitch ermitteln, Alias aus DB ermitteln
            Name = viewer;
        }
    }
}
