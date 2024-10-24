using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;

using System.IO.Compression;

namespace NGM_Client
{
    class SignalRVoiceChat
    {
        HubConnection hubConnectionVoice;

        Dictionary<string, BufferedWaveProvider> audioBufferStreams;
        Dictionary<string, WaveOut> netAudioOutputs;
        WaveInEvent micInput;

        string basePath;
        string userId = "-1";
        string voiceChannelId;


        public SignalRVoiceChat(string _basePath, int _micNumber, string _userId)
        {
            basePath = _basePath;
            userId = _userId;
            micInput = new WaveInEvent();
            micInput.DeviceNumber = _micNumber;
            micInput.WaveFormat = new WaveFormat(44100, 32, 1);//8000, 16, 1
            micInput.DataAvailable += Voice_Input;

            netAudioOutputs = new Dictionary<string, WaveOut>();
            audioBufferStreams = new Dictionary<string, BufferedWaveProvider>();

            //micInput.StartRecording();
        }

        void AddVoiceUser(string id)
        {
            audioBufferStreams[id] = new BufferedWaveProvider(micInput.WaveFormat);
            netAudioOutputs[id] = new WaveOut();
            netAudioOutputs[id].Init(audioBufferStreams[id]);

            netAudioOutputs[id].Play();
        }



        public async Task ConnectToVoiceChat(string _channelId)
        {
            voiceChannelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/voice")
                .Build();


            hubConnectionVoice.On<string, byte[]>("ReceiveVoice", (_userId, data) =>
            {

                ///if (userId == _userId) return;
                if (audioBufferStreams.ContainsKey(_userId))
                    try
                    {
                        byte[] decompressedData = DataWrapper.Decompress(data);
                        audioBufferStreams[_userId].AddSamples(decompressedData, 0, decompressedData.Length);
                    }
                    catch { }
                else AddVoiceUser(_userId);

            });

            await hubConnectionVoice.StartAsync();
            await hubConnectionVoice.InvokeAsync("connect", _channelId, userId);


            micInput.StartRecording();
        }



        public async Task LeaveVoiceChat()
        {

            await hubConnectionVoice.InvokeAsync("disconnect", voiceChannelId, userId);
            await hubConnectionVoice.StopAsync();
            voiceChannelId = "";

            foreach (KeyValuePair<string, WaveOut> entry in netAudioOutputs)
            {
                netAudioOutputs[entry.Key].Stop();
            }
            netAudioOutputs = new Dictionary<string, WaveOut>();
            audioBufferStreams = new Dictionary<string, BufferedWaveProvider>();
            micInput.StopRecording();
        }

        private async void Voice_Input(object sender, WaveInEventArgs e)
        {
            //Console.WriteLine(e.BytesRecorded+" "+ DataWrapper.Compress(e.Buffer).Length + " " + DataWrapper.Decompress(DataWrapper.Compress(e.Buffer)).Length);
            if (hubConnectionVoice != null)
                await hubConnectionVoice.InvokeAsync("SendToVoiceChat", voiceChannelId, userId, DataWrapper.Compress( e.Buffer));
        }
    }
}
