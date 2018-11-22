using Dowsingman2.BaseClass;
using Dowsingman2.Properties;
using System.Windows;
using Dowsingman2.Dialog;

namespace Dowsingman2.SubManager
{
    internal class SettingManager
    {
        private static SettingManager instance_ = new SettingManager();
        public static SettingManager GetInstance() { return instance_; }

        internal Settings Settings { get; private set; }

        private SettingManager()
        {
            Load();
        }

        public int GetVolume()
        {
            return Settings.Volume;
        }

        public Visibility GetButtonVisibility()
        {
            return Settings.IsButtonVisible ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 設定ウィンドウの保存
        /// </summary>
        public void SaveSettingWindow(SettingWindowViewModel settingWindow)
        {
            Settings.IsButtonVisible = settingWindow.IsButtonVisible;
            Settings.Volume = settingWindow.Volume;
            Settings.IsStartupEnable = settingWindow.IsStartupEnable;
        }

        /// <summary>
        /// 設定ウィンドウの復元
        /// </summary>
        public void RecoverSettingWindow(SettingWindowViewModel settingWindow)
        {
            settingWindow.IsButtonVisible = Settings.IsButtonVisible;
            settingWindow.Volume = Settings.Volume;
            settingWindow.IsStartupEnable = Settings.IsStartupEnable;
        }

        /// <summary>
        /// メニューの選択状態を保存
        /// </summary>
        public void SaveViewModel(MainWindowViewModel viewModel)
        {
            Settings.SelectedLeftMenu = viewModel.SelectedLeftMenu.Text;
            Settings.SelectedTopMenu = viewModel.SelectedTopMenu == TopMenuSelection.All;
        }

        /// <summary>
        /// メニューの選択状態を復元
        /// </summary>
        public void RecoverViewModel(MainWindowViewModel viewModel)
        {
            if (viewModel.LeftMenus == null) return;
            if (string.IsNullOrEmpty(Settings.SelectedLeftMenu))
            {
                viewModel.SelectedLeftMenu = viewModel.LeftMenus[0];
            }
            else
            {
                foreach (LeftMenu lm in viewModel.LeftMenus)
                {
                    if (lm.Text == Settings.SelectedLeftMenu)
                    {
                        viewModel.SelectedLeftMenu = lm;
                        lm.IsSelected = true;
                    }
                    else
                    {
                        lm.IsSelected = false;
                    }
                }
            }

            if (Settings.SelectedTopMenu)
            {
                viewModel.SelectedTopMenu = TopMenuSelection.All;
                viewModel.TopMenuFavorite = false;
                viewModel.TopMenuAll = true;
            }
            else
            {
                viewModel.SelectedTopMenu = TopMenuSelection.Favorite;
                viewModel.TopMenuFavorite = true;
                viewModel.TopMenuAll = false;
            }

            viewModel.ButtonVisibility = Settings.IsButtonVisible ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// ウィンドウの位置・サイズを保存します。
        /// </summary>
        public void SaveWindowBounds(MainWindow window)
        {
            Settings.WindowMaximized = window.WindowState == WindowState.Maximized;
            window.WindowState = WindowState.Normal; // 最大化解除
            Settings.WindowLeft = window.Left;
            Settings.WindowTop = window.Top;
            Settings.WindowWidth = window.Width;
            Settings.WindowHeight = window.Height;
        }

        /// <summary>
        /// ウィンドウの位置・サイズを復元します。
        /// </summary>
        public void RecoverWindowBounds(MainWindow window)
        {
            if (Settings.WindowLeft >= 0 &&
                (Settings.WindowLeft + Settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            {
                window.Left = Settings.WindowLeft;
            }
            if (Settings.WindowTop >= 0 &&
                (Settings.WindowTop + Settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            {
                window.Top = Settings.WindowTop;
            }
            if (Settings.WindowWidth >= 640 &&
               Settings.WindowWidth <= SystemParameters.WorkArea.Width)
            {
                window.Width = Settings.WindowWidth;
            }
            if (Settings.WindowHeight >= 420 &&
               Settings.WindowHeight <= SystemParameters.WorkArea.Height)
            {
                window.Height = Settings.WindowHeight;
            }
            if (Settings.WindowMaximized)
            {
                // ロード後に最大化
                window.Loaded += (o, e) => window.WindowState = WindowState.Maximized;
            }
        }

        public void Load()
        {
            Settings = Settings.Default;
        }

        public void Save()
        {
            Settings.Save();
        }
    }
}
