using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Nodes;


namespace NGM_Client
{

    class Program
    {
        
        static async Task Main(string[] args)
        {

            string username, password;
            Console.WriteLine("Username: ");
            username =  Console.ReadLine();
            Console.WriteLine("Password: ");
            password = DataWrapper.GetHashString(Console.ReadLine());

            WClient client = new WClient(username, password);




            /*await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();*/




            Console.WriteLine(await client.VoiceChat().ConnectToVoiceChat("1"));

            string[] com = {""};
            while (com[0]!="exit")
            {
                com=Console.ReadLine().Split();
                if (com[0] == "set_volume") { 
                    client.VoiceChat().SetUserVolume(com[1], float.Parse(com[2], CultureInfo.InvariantCulture) / 2f);
                    Console.WriteLine(string.Format("volume user {0}, changed to {1}", com[1], com[2]));
                }

                if(com[0] == "voice_connect")
                {
                    Console.WriteLine(await client.VoiceChat().ConnectToVoiceChat("1"));
                }

                if (com[0] == "register") 
                {
                    string res;
                    JsonObject tmp=new JsonObject();
                    
                    tmp.Add("id","0");
                    tmp.Add("Username", username);
                    tmp.Add("PasswordHash", password);
                    tmp.Add("Email", "@");
                    tmp.Add("DisplayName", "");


                    res = await client.SendRequest(HttpMethod.Post, "auth/register", tmp, null);//
                    Console.WriteLine(res);
                }
            }

            await client.VoiceChat().LeaveVoiceChat();
                        

            Console.ReadLine();

            

        }


    }
}
