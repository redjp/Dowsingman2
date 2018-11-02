using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Dowsingman2.BaseClass;
using Dowsingman2.SubManager;

namespace Dowsingman2.Dialog
{
    class SettingWindowViewModel : BindableBase
    {
        private int volume_;
        private bool isButtonVisible_;

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

        public ICommand AcceptCommand => new AnotherCommandImplementation(ExcuteCloseWindow4Accept);
        public ICommand CancelCommand => new AnotherCommandImplementation(ExcuteCloseWindow4Cancel);
        public ICommand SoundTestCommand => new AnotherCommandImplementation(ExcuteSoundTest);

        public SettingWindowViewModel()
        {
            SettingManager.GetInstance().RecoverSettingWindow(this);
        }

        #region --AcceptCommand
        private void ExcuteCloseWindow4Accept(object param)
        {
            //設定の保存
            SettingManager.GetInstance().SaveSettingWindow(this);
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
            var filePath = System.IO.Path.GetFullPath(".\\resource\\favorite.wav");
            SoundManager.GetInstance().PlayWaveSound(filePath, Volume);
        }
        #endregion
    }
}
