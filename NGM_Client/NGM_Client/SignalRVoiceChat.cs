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
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace NGM_Client
{
    
    class SignalRVoiceChat
    {

        protected class VoiceUser
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

        protected HubConnection hubConnectionVoice;

        protected WaveInEvent micInput;

        protected string basePath;
        protected string userId = "-1";
        protected string username, password;
        protected string channelId;

        string authHeader;

        bool DirectMessages=false;

        DataWrapper dw;

        Dictionary<string, VoiceUser> voiceUsers;
        VoiceUser voiceUser;

        ScreenCatcher sc;

        System.Windows.Forms.Timer timer1;

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

        public void SetAuthHeader(string a_authHeader)
        {
            authHeader = a_authHeader;
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

            sc = new ScreenCatcher();

            //micInput.StartRecording();

            
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine("e: "+e.BytesRecorded);
            //Console.WriteLine(e.BytesRecorded+" "+ DataWrapper.Compress(e.Buffer).Length + " " + DataWrapper.Decompress(DataWrapper.Compress(e.Buffer)).Length);
            if (hubConnectionVoice != null)
                try
                {
                    await hubConnectionVoice.InvokeAsync("SendScreen", int.Parse(channelId), new byte[1000], DirectMessages);//dw.Encode(e.Buffer)
                    //await hubConnectionVoice.InvokeAsync("SendScreen", int.Parse(channelId), (sc.CaptureScreen()), DirectMessages);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
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


        public async Task<string> ConnectToVoiceChat(string _channelId, bool dm=false)
        {
            DirectMessages = dm;

            if (_channelId == "" || _channelId==null) return "null channelId";
            
            channelId = _channelId;
            hubConnectionVoice = new HubConnectionBuilder()
                .WithUrl(basePath + "hubs/voice", options =>
                {
                    options.Headers["Authorization"] = authHeader;
                }
                )
                .Build();


            hubConnectionVoice.On<UnauthorizedResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });
            hubConnectionVoice.On<BadRequestResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });
            hubConnectionVoice.On<ConflictResult>("ReceiveError", async (message) =>
            {
                Console.WriteLine(message.StatusCode);
            });


            hubConnectionVoice.On<User>("ReceiveUserConnect", async (user) =>
            {
                Console.WriteLine(user.username+" connected");
            });
            hubConnectionVoice.On<User, byte[]>("ReceiveVoice", async (caller, data) =>
            {
                
                OnReceiveVoice(caller.id.ToString(), data);
            });

            hubConnectionVoice.On<User, byte[]>("ReceiveScreen", async (caller, data) =>
            {
                Console.WriteLine(DateTime.Now.ToString());
                //Console.WriteLine(caller.id.ToString());
                OnReceiveScreen(caller.id.ToString(), data);
            });






            try
            {
                Console.WriteLine("connecting: "+channelId+" "+ username+" "+ password);
                await hubConnectionVoice.StartAsync();
                await hubConnectionVoice.InvokeAsync("Connect", int.Parse(channelId), DirectMessages);
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); return ex.Message; }

            micInput.StartRecording();

            var timer = new System.Timers.Timer();
            timer.Interval = 20; // 5 seconds
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            return "connected";
        }

        public void OnReceiveVoice(string a_id, byte[]  data)
        {
            if (userId == a_id) return;
            if (voiceUsers.ContainsKey(a_id))
                try
                {

                    if (voiceUsers[a_id].GetAudioBufferStream().BufferedBytes >= 4800 * 4)
                        voiceUsers[a_id].GetAudioBufferStream().ClearBuffer();


                    byte[] decompressedData = voiceUsers[a_id].Getdw().Decode(data);// 
                    voiceUsers[a_id].GetAudioBufferStream().AddSamples(decompressedData, 0, decompressedData.Length);

                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            else { AddVoiceUser(a_id); }
        }

        public void OnReceiveScreen(string a_id, byte[] data)
        {
            //if (userId == a_id) return;
            Bitmap img= ScreenCatcher.ByteToImage((data));
            string picturesDirectory = "C:\\Oleg\\git\\NGM\\NGM_Client\\NGM_Client\\";
            img.Save(picturesDirectory + "test.png", ImageFormat.Png);
        }

        public async Task LeaveVoiceChat()
        {
            try
            {
                if (channelId == "" || channelId == null) return;

                await hubConnectionVoice.InvokeAsync("Disconnect", int.Parse(channelId));
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

            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine("e: "+e.BytesRecorded);
            //Console.WriteLine(e.BytesRecorded+" "+ DataWrapper.Compress(e.Buffer).Length + " " + DataWrapper.Decompress(DataWrapper.Compress(e.Buffer)).Length);
            if (hubConnectionVoice != null)
                try
                {
                    await hubConnectionVoice.InvokeAsync("Send", int.Parse(channelId), dw.Encode(e.Buffer), DirectMessages);//dw.Encode(e.Buffer)
                    //await hubConnectionVoice.InvokeAsync("SendScreen", int.Parse(channelId), (sc.CaptureScreen()), DirectMessages);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

        }

    }
}
