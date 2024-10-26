using System;
using System.Collections.Generic;


namespace NGM_Client
{

    class Program
    {
        
        static async Task Main(string[] args)
        {

            string username, password;
            Console.WriteLine("Username: ");
            username=Console.ReadLine();
            Console.WriteLine("Password: ");
            password = Console.ReadLine();

            Client client = new Client();

            string res;
            res = await client.SendRequest(HttpMethod.Delete, "Test", new List<string> { "Username", username, "Password", DataWrapper.GetHashString(password) }, "");//
            Console.WriteLine("post: " + res);



            /*await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();*/




            /*await client.VoiceChat().ConnectToVoiceChat(chatId);

            string com = "";
            while (com!="exit")
            {
                com=Console.ReadLine();
                if (com.Split()[0] =="set_volume")client.VoiceChat().SetUserVolume(com.Split()[1], float.Parse(com.Split()[2], CultureInfo.InvariantCulture)/2f);
            }

            await client.VoiceChat().LeaveVoiceChat();
                        

            Console.ReadLine();

            */

        }


    }
}
