using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Dowsingman2.BaseClass
{
    public class LeftMenu : BindableBase
    {
        private bool isSelected_;
        private Visibility topMenuVisibility_;
        private PackIconKind iconKind_;
        private string text_;
        private AbstractManager manager_;

        public bool IsSelected
        {
            get { return isSelected_; }
            set
            {
                if (isSelected_ == value) return;
                isSelected_ = value;
                OnPropertyChanged();
            }
        }

        public Visibility TopMenuVisibility
        {
            get { return topMenuVisibility_; }
            set {
                if (topMenuVisibility_ == value) return;
                topMenuVisibility_ = value;
                OnPropertyChanged();
            }
        }

        public PackIconKind IconKind
        {
            get { return iconKind_; }
            set {
                if (iconKind_ == value) return;
                iconKind_ = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get { return text_; }
            set {
                if (text_ == value) return;
                text_ = value;
                OnPropertyChanged();
            }
        }

        public AbstractManager Manager
        {
            get { return manager_; }
            set {
                if (manager_ == value) return;
                manager_ = value;
                OnPropertyChanged();
            }
        }
    }
}
