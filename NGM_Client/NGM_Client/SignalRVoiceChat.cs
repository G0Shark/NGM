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
        string username, password;
        string channelId;

        DataWrapper dw;

        Dictionary<string, VoiceUser> voiceUsers;
        VoiceUser voiceUser;


        //private
        void AddVoiceUser(string id)
        {
            voiceUsers[id] = new VoiceUser(new BufferedWaveProvider(micInput.WaveFormat), new WaveOutEvent(), new DataWrapper(16000, 16000, 3200, 1));

            voiceUsers[id].netAudioOutput.Init(voiceUsers[id].audioBufferStream);

            var volumeSampleProvider = new VolumeSampleProvider(voiceUsers[id].audioBufferStream.ToSampleProvider());
            volumeSampleProvider.Volume = 2.0f;

            voiceUsers[id].netAudioOutput.Init(volumeSampleProvider);
            voiceUsers[id].netAudioOutput.Volume = 0.5f;

            voiceUsers[id].netAudioOutput.Play();
        }
        //getters

        //setters
        public void SetUserId(string _val)
        {
            userId = _val;
        }
        public void SetUsername(string _val)
        {
            username = _val;
        }
        public void SetPassword(string _val)
        {
            password = _val;
        }

        //public
        public SignalRVoiceChat(string _basePath, int _micNumber, string _username = "username", string _password = "password", string _userId = "-1")
        {
            basePath = _basePath;
            userId = _userId;
            username = _username;
            password = _password;
            micInput = new WaveInEvent();
            micInput.DeviceNumber = _micNumber;
            micInput.WaveFormat = new WaveFormat(16000, 1);//8000, 16, 1  new WaveFormat(8000, 32, 1);
            micInput.DataAvailable += Voice_Input;

            voiceUsers = new Dictionary<string, VoiceUser>();

            dw = new DataWrapper(16000, 16000, 3200, 1);

            //micInput.StartRecording();
        }

        


        public void SetUserVolume(string uid, float val)
        {
            if(voiceUsers.ContainsKey(uid))
                voiceUsers[uid].netAudioOutput.Volume = val;
        }


        public async Task<string> ConnectToVoiceChat(string _channelId)
        {
            if (_channelId == "") return "null channelId";
            
            channelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/voice")
                .Build();

            hubConnectionVoice.On<string>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message);
            });
            hubConnectionVoice.On<string, byte[]>("ReceiveVoice", async (_username, data) =>
            {

                if (username == _username) return;
                if (voiceUsers.ContainsKey(_username))
                    try
                    {
                        //Console.WriteLine(audioBufferStreams[_userId].BufferedDuration);
                        if (voiceUsers[_username].audioBufferStream.BufferedBytes>=6400)
                            voiceUsers[_username].audioBufferStream.ClearBuffer();


                        byte[] decompressedData = voiceUsers[_username].dw.Decode(data);// 
                        voiceUsers[_username].audioBufferStream.AddSamples(decompressedData, 0, decompressedData.Length);


                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                else { AddVoiceUser(_username); }

            });



            try
            {
                Console.WriteLine(channelId+" "+ username+" "+ password);
                await hubConnectionVoice.StartAsync();
                await hubConnectionVoice.InvokeAsync("connect", channelId, username, password);
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); return ex.Message; }

            micInput.StartRecording();

            return "connected";
        }



        public async Task LeaveVoiceChat()
        {
            if (channelId == "") return;

            await hubConnectionVoice.InvokeAsync("disconnect", channelId, username, password);
            await hubConnectionVoice.StopAsync();
            channelId = "";

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
                try
                { 
                    await hubConnectionVoice.InvokeAsync("SendToVoiceChat", channelId, username, dw.Encode(e.Buffer));//dw.Encode(e.Buffer)
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

        }



    }
}
