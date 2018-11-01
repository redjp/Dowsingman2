using Dowsingman2.Properties;
using System.Windows;

namespace Dowsingman2.SubManager
{
    public class SettingManager
    {
        private static SettingManager instance_ = new SettingManager();
        public static SettingManager GetInstance() { return instance_; }

        internal Settings Settings { get; private set; }

        private SettingManager()
        {
            Settings = Settings.Default;
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
            Settings.SelectedTabIndex = window.TabControl1.SelectedIndex;
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
            if (Settings.WindowWidth >= 600 &&
               Settings.WindowWidth <= SystemParameters.WorkArea.Width)
            {
                window.Width = Settings.WindowWidth;
            }
            if (Settings.WindowHeight >= 360 &&
               Settings.WindowHeight <= SystemParameters.WorkArea.Height)
            {
                window.Height = Settings.WindowHeight;
            }
            if (Settings.WindowMaximized)
            {
                // ロード後に最大化
                window.Loaded += (o, e) => window.WindowState = WindowState.Maximized;
            }
            if (Settings.SelectedTabIndex >= 0)
            {
                window.TabControl1.SelectedIndex = Settings.SelectedTabIndex;
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
