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



            ScreenCatcher sc=new ScreenCatcher();
            sc.CaptureScreen();


            string username, password;
            Console.WriteLine("Username: ");
            username =  Console.ReadLine();
            //Console.WriteLine("Password: ");
            password = Console.ReadLine();


            Client client = new Client(username, password);
            
            //Member[] aserverMembers = await client.GetServerMembers("3");
            //Console.WriteLine(aserverMembers.Length);


            string res;
            string[] com = {"","", "", "", "", "", "", "", "", "" };
            while (com[0] != "exit")
            {
                string[] data = Console.ReadLine().Split();

                for (int i = 0; i < data.Length; i++)
                    com[i] = data[i];

                bool dm;

                switch (com[0])
                {
                    case "auth":
                        switch (com[1])
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
                            case "delete":
                                res = (await client.Delete());//
                                Console.WriteLine(res);
                                break;
                            case "get":
                                res = (await client.GetUserById(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;
                            case "getu":
                                res = (await client.GetByUsername(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;
                        }
                        break;



                    case "server":
                        switch (com[1])
                        {
                            case "create":
                                res = await client.ServerCreate(com[2], com[3], com[4], true);
                                Console.WriteLine(res);
                                break;
                            case "delete":
                                res = await client.ServerDelete(com[2]);
                                Console.WriteLine(res);
                                break;
                            case "change":
                                res = await client.ServerChange(com[2], (ServerParams)Enum.Parse(typeof(ServerParams), com[3]), com[4]);
                                Console.WriteLine(res);
                                break;
                            case "get":
                                res = (await client.GetServerById(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;
                            case "geta":
                                res = (await client.GetServers())[0].id.ToString();
                                Console.WriteLine(res);
                                break;
                        }
                        break;
                    case "member":
                        switch (com[1])
                        {
                            case "join":
                                res = await client.MemberJoin(com[2], com[3]);
                                Console.WriteLine(res);
                                break;
                            case "leave":
                                res = await client.MemberLeave(com[2]);
                                Console.WriteLine(res);
                                break;
                            case "mute":
                                res = await client.MemberMute((MuteParams)Enum.Parse(typeof(MuteParams),com[2]), com[3], com[4], com[5]=="true");
                                Console.WriteLine(res);
                                break;
                            case "get":
                                res = (await client.GetMemberById(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;

                        }
                        break;

                    case "role":
                        switch (com[1])
                        {
                            case "create":
                                res = await client.RoleCreate(com[2], com[3], com[4], com[5]);
                                Console.WriteLine(res);
                                break;
                            case "delete":
                                res = await client.RoleDelete(com[2]);
                                Console.WriteLine(res);
                                break;
                            case "change":
                                res = await client.RoleChange(com[2], (RoleParams)Enum.Parse(typeof(RoleParams), com[3]), com[4]);
                                Console.WriteLine(res);
                                break;
                            case "get":
                                res = (await client.GetRoleById(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;

                        }
                        break;

                    case "chat":
                        switch (com[1])
                        {
                            case "create":
                                res = await client.CteateChat(com[2], com[3], com[4], com[5], com[6]);
                                Console.WriteLine(res);
                                break;
                            case "delete":
                                res = await client.DeleteChat(com[2]);
                                Console.WriteLine(res);
                                break;
                            case "get":
                                res = (await client.GetChatById(com[2])).id.ToString();
                                Console.WriteLine(res);
                                break;

                        }
                        break;

                    case "voice":
                        switch (com[1])
                        {
                            case "connect":
                                dm = (com[3] == "dm");
                                await client.GetWClient().VoiceChat().ConnectToVoiceChat(com[2],dm);
                                break;
                            case "leave":
                                await client.GetWClient().VoiceChat().LeaveVoiceChat();
                                Console.WriteLine("done");
                                break;

                            case "set_volume":
                                client.GetWClient().VoiceChat().SetUserVolume(com[2], float.Parse(com[3], CultureInfo.InvariantCulture));
                                Console.WriteLine("done");
                                break;
                        }
                        break;
                    case "text":
                        switch (com[1])
                        {
                            case "connect":
                                dm = (com[3] == "dm");
                                string chat_id = com[2];

                                await client.GetWClient().TextChat().ConnectToTextChat(new List<string> { com[2] }, dm);


                                Console.Clear();

                                Console.WriteLine("connected");



                                Message[] messages = await client.GetMessages(chat_id, dm);

                                Member[] serverMembers;
                                Dictionary<int, string> id_members = new Dictionary<int, string>();



                                if(!dm)
                                {
                                    string server_id = (await client.GetChatById(chat_id)).serverid.ToString();
                                    Console.WriteLine(server_id);
                                    serverMembers = await client.GetServerMembers(server_id);

                                    for (int i = 0; i < serverMembers.Length; i++)
                                        id_members[serverMembers[i].userid] = serverMembers[i].displayname;
                                }
                                id_members[int.Parse(chat_id)] = (await client.GetUserById(chat_id)).displayname;

                                foreach (Message mes in messages)
                                {
                                    if(!dm)
                                        Console.WriteLine(id_members[mes.senderid] + ": " + mes.message);
                                }
                                    
                                    

                                string m = Console.ReadLine();
                                while (m != "exit")
                                {
                                    client.GetWClient().TextChat().SendToTextChat(chat_id, m);
                                    m = Console.ReadLine();
                                }
                                await client.GetWClient().TextChat().LeaveTextChat(com[2]);
                                Console.WriteLine("done");

                                break;

                        }
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
