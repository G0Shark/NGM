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
            //Console.WriteLine("Password: ");
            password = DataWrapper.GetHashString(Console.ReadLine());


            Client client = new Client(username, password);


            string res;
            string[] com = { "" };
            while (com[0] != "exit")
            {
                com = Console.ReadLine().Split();

                switch (com[0])
                {
                    case "login":
                        {
                            try
                            {
                                User user = (await client.Login());
                                Console.WriteLine(user.id);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        break;
                    case "register":
                        {
                            res = (await client.Register(["DESCRIPTION", "Oleg", 
                                "USERNAME", username, 
                                "PASSWORD", password, 
                                "EMAIL", "omegadroid@yandex.com", 
                                "DISPLAYNAME", username]));//
                            Console.WriteLine(res);
                        }
                        break;
                    case "get_messages":
                        Message[] messages = await client.GetMessages(com[1],false);
                        Console.WriteLine(messages[0].message);
                        break;
                    case "makeship":
                        res = await client.FriendshipAction(FriendshipActions.make, com[1]);
                        Console.WriteLine(res);
                        break;
                    case "sendreq":
                        res = await client.FriendshipAction(FriendshipActions.sendreq, com[1]);
                        Console.WriteLine(res);
                        break;
                    case "blockship":
                        res = await client.FriendshipAction(FriendshipActions.block, com[1]);
                        Console.WriteLine(res);
                        break;
                    case "unblockship":
                        res = await client.FriendshipAction(FriendshipActions.unblock, com[1]);
                        Console.WriteLine(res);
                        break;


                    case "role_create":
                        res = await client.RoleCreate(com[1], com[2], com[3], com[4]);
                        Console.WriteLine(res);
                        break;
                    case "role_delete":
                        res = await client.RoleDelete(com[1]);
                        Console.WriteLine(res);
                        break;
                    case "role_change":
                        res = await client.RoleChange(com[1], (RoleParams)Enum.Parse(typeof(RoleParams), com[2]), com[3]);
                        Console.WriteLine(res);
                        break;


                    case "server_create":
                        res = await client.ServerCreate(com[1], com[2], com[3], true);
                        Console.WriteLine(res);
                        break;
                    case "server_delete":
                        res = await client.ServerDelete(com[1]);
                        Console.WriteLine(res);
                        break;
                    case "server_change":
                        res = await client.ServerChange(com[1], (ServerParams)Enum.Parse(typeof(ServerParams), com[2]), com[3]);
                        Console.WriteLine(res);
                        break;
                }
            }


            
            

            /*
            await client.ConnectToTextChat(new List<string>(){ chatId });
            string mes = "";
            do
            {
                mes = Console.ReadLine();
                await client.SendToTextChat(chatId,mes);
            }
            while (mes != "exit");
            await client.LeaveTextChat();//*/




            //Console.WriteLine(await client.VoiceChat().ConnectToVoiceChat("1"));

            /*
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
            */

            

        }


    }
}
