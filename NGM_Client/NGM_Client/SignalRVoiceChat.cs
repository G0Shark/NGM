using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using System.IO.Compression;
using NAudio.Wave.SampleProviders;
using System.Reflection.PortableExecutable;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NGM_Client
{
    class SignalRVoiceChat
    {

        struct VoiceUser
        {
            public BufferedWaveProvider audioBufferStream { get; set; }
            public WaveOutEvent netAudioOutput { get; set; }
            public DataWrapper dw { get; set; }

            public VoiceUser(BufferedWaveProvider _audioBufferStream, WaveOutEvent _netAudioOutput, DataWrapper _dw)//
            {
                audioBufferStream = _audioBufferStream;
                netAudioOutput = _netAudioOutput;
                dw = _dw;
            }
        };


        HubConnection hubConnectionVoice;

        WaveInEvent micInput;

        string basePath;
        string userId = "-1";
        string voiceChannelId;

        DataWrapper dw;

        Dictionary<string, VoiceUser> voiceUsers;
        VoiceUser voiceUser;




        public SignalRVoiceChat(string _basePath, int _micNumber, string _userId="-1")
        {
            basePath = _basePath;
            userId = _userId;
            micInput = new WaveInEvent();
            micInput.DeviceNumber = _micNumber;
            micInput.WaveFormat = new WaveFormat(16000, 1);//8000, 16, 1  new WaveFormat(8000, 32, 1);
            micInput.DataAvailable += Voice_Input;

            voiceUsers = new Dictionary<string, VoiceUser>();

            dw = new DataWrapper(16000, 16000, 3200, 1);

            //micInput.StartRecording();
        }

        void AddVoiceUser(string id)
        {
            voiceUsers[id] = new VoiceUser(new BufferedWaveProvider(micInput.WaveFormat), new WaveOutEvent(), new DataWrapper(16000, 16000, 3200, 1));

            voiceUsers[id].netAudioOutput.Init(voiceUsers[id].audioBufferStream);

            voiceUsers[id].netAudioOutput.Play();            
        }


        public void SetUserVolume(string uid, float val)
        {
            if(voiceUsers.ContainsKey(uid))
                voiceUsers[uid].netAudioOutput.Volume = val;
        }


        public async Task ConnectToVoiceChat(string _channelId)
        {
            voiceChannelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/voice")
                .Build();


            hubConnectionVoice.On<string, byte[]>("ReceiveVoice", async (_userId, data) =>
            {

                if (userId == _userId) return;
                if (voiceUsers.ContainsKey(_userId))
                    try
                    {
                        //Console.WriteLine(audioBufferStreams[_userId].BufferedDuration);
                        if (voiceUsers[_userId].audioBufferStream.BufferedBytes>=6400)
                            voiceUsers[_userId].audioBufferStream.ClearBuffer();


                        byte[] decompressedData = voiceUsers[_userId].dw.Decode(data);// 
                        voiceUsers[_userId].audioBufferStream.AddSamples(decompressedData, 0, decompressedData.Length);


                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                else { AddVoiceUser(_userId); AddVoiceUser(_userId+"1"); }

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

            foreach (KeyValuePair<string, VoiceUser> entry in voiceUsers)
            {
                voiceUsers[entry.Key].netAudioOutput.Stop();
            }

            voiceUsers = new Dictionary<string, VoiceUser>();
            micInput.StopRecording();
        }

        private async void Voice_Input(object sender, WaveInEventArgs e)
        {


            //Console.WriteLine(e.BytesRecorded);
            //Console.WriteLine(e.BytesRecorded+" "+ DataWrapper.Compress(e.Buffer).Length + " " + DataWrapper.Decompress(DataWrapper.Compress(e.Buffer)).Length);
            if (hubConnectionVoice != null)
                await hubConnectionVoice.InvokeAsync("SendToVoiceChat", voiceChannelId, userId, dw.Encode(e.Buffer));//dw.Encode(e.Buffer)

        }



    }
}
