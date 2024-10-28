using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
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


            Client client = new Client(username, password);

            try
            {
                User user = (await client.Login());
                Console.WriteLine(user.id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            


            /*await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();*/




            //Console.WriteLine(await client.VoiceChat().ConnectToVoiceChat("1"));
            string res;
            string[] com = {""};
            while (com[0]!="exit")
            {
                com=Console.ReadLine().Split();

                switch(com[0])
                {
                    case "set_volume":
                        client.GetWClient().VoiceChat().SetUserVolume(com[1], float.Parse(com[2], CultureInfo.InvariantCulture));
                        Console.WriteLine(string.Format("volume user {0}, changed to {1}", com[1], com[2]));
                        break;
                    case "voice_connect":
                        Console.WriteLine(await client.GetWClient().VoiceChat().ConnectToVoiceChat("1"));
                        break;
                    case "register":
                        {
                            JsonObject tmp = new JsonObject();

                            tmp.Add("id", "0");
                            tmp.Add("Username", username);
                            tmp.Add("PasswordHash", password);
                            tmp.Add("Email", "@");
                            tmp.Add("DisplayName", "");


                            res = (await client.Register(tmp)).id;//
                            Console.WriteLine(res);
                        }
                        break;
                    case "ping":
                        Console.WriteLine(await client.GetPing());
                        break;
                    case "test":
                        string _res = await client.MakeNewPartnership("18");
                        Console.WriteLine(_res);
                        break;
                }
            }

            await client.GetWClient().VoiceChat().LeaveVoiceChat();
                        

            

        }


    }
}
