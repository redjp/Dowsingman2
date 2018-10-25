using System.IO;
using System.Threading;

namespace Dowsingman2
{
    using System.Windows;

    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private NotifyIconWrapper notifyIcon;

        private static Mutex mutex;

        private const string APPNAME_GUID = "Dowsingman2-{FAB552E3-0F8F-446F-90A0-EE05BDE8C8D1}";

        /// <summary>
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            bool new_mutex;
            mutex = new Mutex(false, APPNAME_GUID, out new_mutex);
            if (!new_mutex)
            {
                MessageBox.Show("すでに起動しています！");
                mutex.Close();
                mutex = null;
                this.Shutdown();
                return;
            }

            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();
        }

        /// <summary>
        /// System.Windows.Application.Exit イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            mutex?.Close();

            base.OnExit(e);

            this.notifyIcon?.Dispose();
        }
    }
}
