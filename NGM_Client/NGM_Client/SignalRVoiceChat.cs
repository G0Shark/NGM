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
using System.Timers;
using System.Diagnostics;
using System.Security.Cryptography;

namespace NGM_Client
{
    class SignalRVoiceChat
    {

        class VoiceUser
        {
             BufferedWaveProvider audioBufferStream;
             WaveOutEvent netAudioOutput;
             DataWrapper dw;
             float volume  = 1;

            public void SetVolume (float _volume) { volume = _volume; }
            public void SetAudioBufferStream(BufferedWaveProvider _audioBufferStream) { audioBufferStream = _audioBufferStream; }
            public void SetNetAudioOutput(WaveOutEvent _netAudioOutput) { netAudioOutput = _netAudioOutput; }
            public void Setdw(DataWrapper _dw) { dw = _dw; }

            public ref float GetVolume() { return ref volume; }
            public ref BufferedWaveProvider GetAudioBufferStream() { return ref audioBufferStream; }
            public ref WaveOutEvent GetNetAudioOutput() { return ref netAudioOutput; }
            public ref DataWrapper Getdw() { return ref dw; }

            public VoiceUser(BufferedWaveProvider _audioBufferStream, WaveOutEvent _netAudioOutput, DataWrapper _dw)//
            {
                audioBufferStream = _audioBufferStream;
                netAudioOutput = _netAudioOutput;
                dw = _dw;
            }

            public void SetUserVolume(float val)
            {
                
                volume= val;
                netAudioOutput.Stop();
                netAudioOutput=new WaveOutEvent();

                var volumeSampleProvider = new VolumeSampleProvider(audioBufferStream.ToSampleProvider());
                volumeSampleProvider.Volume = val;
                netAudioOutput.Init(volumeSampleProvider);

                netAudioOutput.Play();
                
                

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
            voiceUsers[id] = new VoiceUser(new BufferedWaveProvider(micInput.WaveFormat), new WaveOutEvent(), new DataWrapper(24000, 0, 4800, 1));// 24

            

            var volumeSampleProvider = new VolumeSampleProvider(voiceUsers[id].GetAudioBufferStream().ToSampleProvider());
            volumeSampleProvider.Volume = 2.0f;

            //voiceUsers[id].netAudioOutput.Init(voiceUsers[id].audioBufferStream);
            voiceUsers[id].GetNetAudioOutput().Init(volumeSampleProvider);
            voiceUsers[id].GetNetAudioOutput().Volume = 0.5f;

            voiceUsers[id].GetNetAudioOutput().Play();
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
            micInput.WaveFormat = new WaveFormat(24000, 1);//8000, 16, 1  new WaveFormat(8000, 32, 1);
            micInput.DataAvailable += Voice_Input;

            voiceUsers = new Dictionary<string, VoiceUser>();

            dw = new DataWrapper(24000, 0, 4800, 1);

            //micInput.StartRecording();
        }

        


        public void SetUserVolume(string uid, float val)
        {
            if (voiceUsers.ContainsKey(uid))
            voiceUsers[uid].SetUserVolume(val);

            /*
             * Console.WriteLine(uid + ": " + voiceUsers[uid].GetVolume() + " " + uid + val);
            foreach (KeyValuePair<string, VoiceUser> p in voiceUsers)
                Console.WriteLine(p.Key + ": " + p.Value.GetVolume());
            */
        }


        public async Task<string> ConnectToVoiceChat(string _channelId)
        {
            if (_channelId == "" || _channelId==null) return "null channelId";
            
            channelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hub/voice")
                .Build();

            hubConnectionVoice.On<string>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message);
            });
            hubConnectionVoice.On<string, string>("ReceiveMessage", async (username, message) =>
            {
                Console.WriteLine(username+": "+message);
            });
            hubConnectionVoice.On<string, byte[]>("ReceiveVoice", async (_username, data) =>
            {
                if (username == _username) return;
                if (voiceUsers.ContainsKey(_username))
                    try
                    {

                        if (voiceUsers[_username].GetAudioBufferStream().BufferedBytes >= 4800*4)
                            voiceUsers[_username].GetAudioBufferStream().ClearBuffer();


                        byte[] decompressedData = voiceUsers[_username].Getdw().Decode(data);// 
                        voiceUsers[_username].GetAudioBufferStream().AddSamples(decompressedData, 0, decompressedData.Length);

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
            try
            {
                if (channelId == "" || channelId == null) return;

                await hubConnectionVoice.InvokeAsync("disconnect", channelId, username, password);
                await hubConnectionVoice.StopAsync();
                channelId = "";

                foreach (KeyValuePair<string, VoiceUser> entry in voiceUsers)
                {
                    voiceUsers[entry.Key].GetNetAudioOutput().Stop();
                }

                voiceUsers = new Dictionary<string, VoiceUser>();
                micInput.StopRecording();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }

        private async void Voice_Input(object sender, WaveInEventArgs e)
        {


            //Console.WriteLine("e: "+e.BytesRecorded);
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
