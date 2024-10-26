using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System.IO.Compression;
using Concentus;
using Concentus.Structs;
using NAudio.Wave.SampleProviders;

namespace NGM_Client
{
    class SignalRVoiceChat
    {
        HubConnection hubConnectionVoice;

        Dictionary<string, BufferedWaveProvider> audioBufferStreams;
        Dictionary<string, WaveOutEvent> netAudioOutputs;
        WaveInEvent micInput;

        string basePath;
        string userId = "-1";
        string voiceChannelId;

        DataWrapper dw;
        OpusEncoder encoder;
        OpusDecoder decoder;

        public SignalRVoiceChat(string _basePath, int _micNumber, string _userId)
        {
            basePath = _basePath;
            userId = _userId;
            micInput = new WaveInEvent();
            micInput.DeviceNumber = _micNumber;
            micInput.WaveFormat = new WaveFormat(16000, 1);//8000, 16, 1  new WaveFormat(8000, 32, 1);
            micInput.DataAvailable += Voice_Input;

            netAudioOutputs = new Dictionary<string, WaveOutEvent>();
            audioBufferStreams = new Dictionary<string, BufferedWaveProvider>();

            dw = new DataWrapper(16000, 16000, 320, 1);

            //micInput.StartRecording();
        }

        void AddVoiceUser(string id)
        {
            audioBufferStreams[id] = new BufferedWaveProvider(micInput.WaveFormat);
            netAudioOutputs[id] = new WaveOutEvent();

            var volumeSampleProvider = new VolumeSampleProvider(audioBufferStreams[id].ToSampleProvider());
            volumeSampleProvider.Volume = 2.0f;
            netAudioOutputs[id].Init(volumeSampleProvider);
            netAudioOutputs[id].Volume = 0.5f;
            netAudioOutputs[id].Play();
        }


        public void SetUserVolume(string uid, float val)
        {
            if(netAudioOutputs.ContainsKey(uid))
                netAudioOutputs[uid].Volume = val;
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
                        
                        byte[] decompressedData = dw.Decode (data);
                        audioBufferStreams[_userId].AddSamples(decompressedData, 0, decompressedData.Length);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
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

            foreach (KeyValuePair<string, WaveOutEvent> entry in netAudioOutputs)
            {
                netAudioOutputs[entry.Key].Stop();
            }
            netAudioOutputs = new Dictionary<string, WaveOutEvent>();
            audioBufferStreams = new Dictionary<string, BufferedWaveProvider>();
            micInput.StopRecording();
        }

        private async void Voice_Input(object sender, WaveInEventArgs e)
        {


            Console.WriteLine(e.BytesRecorded);
            //Console.WriteLine(e.BytesRecorded+" "+ DataWrapper.Compress(e.Buffer).Length + " " + DataWrapper.Decompress(DataWrapper.Compress(e.Buffer)).Length);
            if (hubConnectionVoice != null)
                await hubConnectionVoice.InvokeAsync("SendToVoiceChat", voiceChannelId, userId, dw.Encode(e.Buffer));//

        }



    }
}
