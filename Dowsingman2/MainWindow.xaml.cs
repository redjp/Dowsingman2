using Dowsingman2.BaseClass;
using Dowsingman2.SubManager;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Dowsingman2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            SettingManager.GetInstance().RecoverWindowBounds(this);

            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
        }

        /// <summary>
        /// ContextMenuOpening用イベントハンドラ
        /// </summary>
        private void HandlerForCMO(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement fe = e.Source as FrameworkElement;
            fe.ContextMenu = BuildMenu();

        }

        /// <summary>
        /// ContextMenuの作成
        /// </summary>
        private ContextMenu BuildMenu()
        {
            ContextMenu theMenu = new ContextMenu();

            if (!Equals(viewModel.SelectedLeftMenu.TopMenuVisibility, Visibility.Visible)) return theMenu;

            if (Equals(viewModel.SelectedTopMenu, TopMenuSelection.Favorite))
            {
                theMenu.Items.Add(new MenuItem
                {
                    Header = "新しく追加",
                    Command = viewModel.AddButtonCommand
                });

                if (viewModel.SelectedData == null) return theMenu;

                theMenu.Items.Add(new MenuItem
                {
                    Header = viewModel.SelectedData.Owner + "を削除",
                    Command = viewModel.DeleteButtonCommand,
                    CommandParameter = viewModel.SelectedData
                });
            }
            else if (viewModel.SelectedData != null)
            { 
                theMenu.Items.Add(new MenuItem
                {
                    Header = viewModel.SelectedData.Owner + "を追加",
                    Command = viewModel.AddButtonCommand,
                    CommandParameter = viewModel.SelectedData
                });
            }

            return theMenu;
        }

        /// <summary>
        /// 閉じるときにウィンドウの情報を保存
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SettingManager.GetInstance().SaveWindowBounds(this);
        }
    }
}
