using Dowsingman2.UtilityClass;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace Dowsingman2.SubManager
{
    internal class SoundManager
    {
        private static SoundManager instance_ = new SoundManager();
        public static SoundManager GetInstance() { return instance_; }

        private WaveOutEvent waveOut_;
        private WaveChannel32 waveChannel_;

        public bool IsPlaying { get; set; }

        /// <summary>
        /// サウンドの再生
        /// </summary>
        public void PlayWaveSound(string filePath, int volume)
        {
            if (IsPlaying) return;
            IsPlaying = true;

            try
            {
                waveOut_ = new WaveOutEvent();
                waveChannel_ = new WaveChannel32(new WaveFileReader(filePath));
                waveChannel_.Volume = volume / 100.0f;
                waveChannel_.PadWithZeroes = false;
                waveOut_.Init(waveChannel_);

                waveOut_.PlaybackStopped += OnPlaybackStopped;

                waveOut_.Play();
            }
            catch (Exception ex)
            {
                MyTraceSource.TraceEvent(TraceEventType.Error, ex);
                ClearAll();
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            ClearAll();
        }

        private void ClearAll()
        {
            waveChannel_?.Dispose();
            waveOut_?.Dispose();
            waveOut_ = null;

            IsPlaying = false;
        }
    }
}
