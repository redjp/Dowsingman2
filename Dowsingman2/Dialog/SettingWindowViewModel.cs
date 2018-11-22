using Dowsingman2.BaseClass;
using Dowsingman2.SubManager;
using Dowsingman2.UtilityClass;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Dowsingman2.Dialog
{
    class SettingWindowViewModel : BindableBase
    {
        private int volume_;
        private bool isButtonVisible_;
        private bool isStartupEnable_;

        #region
        public int Volume
        {
            get { return volume_; }
            set
            {
                if (volume_ == value) return;
                volume_ = value;
                OnPropertyChanged();
            }
        }

        public bool IsButtonVisible
        {
            get { return isButtonVisible_; }
            set
            {
                if (isButtonVisible_ == value) return;
                isButtonVisible_ = value;
                OnPropertyChanged();
            }
        }

        public bool IsStartupEnable
        {
            get { return isStartupEnable_; }
            set {
                if (isStartupEnable_ == value) return;
                isStartupEnable_ = value;
                OnPropertyChanged();
                IsStartupChanged = true;
            }
        }

        public bool IsStartupChanged { get; private set; }
        public string ShortcutName { get; }
        #endregion

        public ICommand AcceptCommand => new AnotherCommandImplementation(ExcuteCloseWindow4Accept);
        public ICommand CancelCommand => new AnotherCommandImplementation(ExcuteCloseWindow4Cancel);
        public ICommand SoundTestCommand => new AnotherCommandImplementation(ExcuteSoundTest);

        public SettingWindowViewModel()
        {
            SettingManager.GetInstance().RecoverSettingWindow(this);
            IsStartupChanged = false;
            ShortcutName = "Dowsingman2.lnk";
        }

        #region --AcceptCommand
        private void ExcuteCloseWindow4Accept(object param)
        {
            //設定の保存
            SettingManager.GetInstance().SaveSettingWindow(this);
            if (IsStartupChanged) AddStartup(IsStartupEnable);
            (param as Window)?.Close();
        }
        #endregion

        #region --CancelCommand
        private void ExcuteCloseWindow4Cancel(object param)
        {
            (param as Window)?.Close();
        }
        #endregion

        #region --SoundTestCommand
        private void ExcuteSoundTest(object o)
        {
            var filePath = "resource\\favorite.wav";
            SoundManager.GetInstance().PlayWaveSound(filePath, Volume);
        }
        #endregion

        /// <summary>
        /// スタートアップフォルダにショートカットを追加
        /// https://www.osadasoft.com/c-%E3%82%B9%E3%82%BF%E3%83%BC%E3%83%88%E3%82%A2%E3%83%83%E3%83%97%E3%83%A1%E3%83%8B%E3%83%A5%E3%83%BC%E3%81%AB%E3%82%B7%E3%83%A7%E3%83%BC%E3%83%88%E3%82%AB%E3%83%83%E3%83%88%E3%82%92%E7%99%BB/
        /// </summary>
        private void AddStartup(bool isEnable)
        {
            var startupPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Startup);
            var shortcutPath = startupPath + "\\" + ShortcutName;

            if (isEnable)
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカットの作成を開始").ToString());
#endif
                var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

                // 参考URL
                // https://stackoverflow.com/questions/234231/creating-application-shortcut-in-a-directory
                Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
                dynamic shell = Activator.CreateInstance(t);
                try
                {
                    var lnk = shell.CreateShortcut(shortcutPath);
                    try
                    {
                        lnk.TargetPath = appPath;
                        lnk.IconLocation = appPath + ", 0";
                        lnk.Save();
                    }
                    finally
                    {
                        Marshal.FinalReleaseComObject(lnk);
                    }
                }
                finally
                {
                    Marshal.FinalReleaseComObject(shell);
                }
#if DEBUG
                if (File.Exists(shortcutPath))
                {
                    MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカット作成成功").ToString());
                }
                else
                {
                    MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカット作成失敗").ToString());
                }
#endif
            }
            else
            {
#if DEBUG
                MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカットの削除を開始").ToString());
#endif
                if (File.Exists(shortcutPath))
                {
                    try
                    {
                        File.Delete(shortcutPath);
                    }
                    catch (IOException ex)
                    {
                        MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                    }
#if DEBUG
                    if (File.Exists(shortcutPath))
                    {
                        MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカット削除失敗").ToString());
                    }
                    else
                    {
                        MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカット削除成功").ToString());
                    }
#endif
                }
                else
                {
#if DEBUG
                    MyTraceSource.TraceEvent(TraceEventType.Information, new StringBuilder(40).Append("[SettingWindow] ショートカットが既に存在しない").ToString());
#endif
                }
            }
        }
    }
}
