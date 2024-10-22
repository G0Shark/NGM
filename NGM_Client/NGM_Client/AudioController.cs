using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
namespace NGM_Client
{
    class AudioController
    {

        private WaveInEvent waveSource;
        private MemoryStream audioStream;
        int micNumber = 0;
        int delay;

        public AudioController(int _micNumber, int _delay)
        {
            micNumber = _micNumber;
            delay = _delay;
        }

        public MemoryStream GetAudioStream()
        {
            return audioStream;
        }

         public void StartRecording()
        {

            audioStream = new MemoryStream();
            waveSource = new WaveInEvent();
            waveSource.WaveFormat = new WaveFormat(32000, 16, 1); // 44.1kHz, Mono
            waveSource.DeviceNumber = deviceNumber;
            waveSource.DataAvailable += OnDataAvailable;
            waveSource.StartRecording();
        }

         private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            audioStream.Write(e.Buffer, 0, e.BytesRecorded);
        }

         public void StopRecording()
        {
            waveSource.StopRecording();
            waveSource.Dispose();
        }

         public void PlayAudio(byte[] audioBytes, int sampleRate)
        {
            using (var ms = new MemoryStream(audioBytes))
            {
                // Create a RawSourceWaveStream from the byte array
                using (var waveStream = new RawSourceWaveStream(ms, new WaveFormat(sampleRate, 16, 1))) // 16-bit mono
                {
                    using (var waveOut = new WaveOutEvent())
                    {
                        waveOut.Init(waveStream);
                        waveOut.Play();

                        // Wait for playback to finish
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(delay);
                        }
                    }
                }
            }
        }
    }
}
