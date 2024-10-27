using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;
using Org.BouncyCastle.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainChatController)]
public class ChatController : Controller
{
    /*[HttpPatch("addtochat")]
    public ActionResult AddToChat(string username, string password, string chatid, string other_username)
    {
        
    }*/
    [HttpPost(Path.CreateChat)]
    public ActionResult CreateChat(string channelName, string channelDescription, string channelType)
    {
        try
        {
            if (Request.Headers.TryGetValue("Username", out var username) &&
                Request.Headers.TryGetValue("Password", out var password))
            {
                if (!Secure.TryLogin(username!, password!))
                {
                    return Unauthorized("Неверный логин\\пароль.");
                }

                if (channelName == "" && channelType == "")
                {
                    return BadRequest("channelName или channelType не указаны.");
                }

                Channel c = new Channel();

                MySqlConnection conn = new MySqlConnection(gb.dbconn);
                conn.Open();
                using (MySqlCommand cmd =
                       new MySqlCommand(
                           "INSERT INTO `chats`(`channelName`, `channelDescription`, `type`) VALUES (@cn,@cd,@ct)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("@cn", channelName);
                    cmd.Parameters.AddWithValue("@cd", channelDescription);
                    cmd.Parameters.AddWithValue("@ct", channelType);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c = new Channel
                            {
                                Id = reader[0].ToString() ?? "null",
                                channelName = reader[1].ToString() ?? "null",
                                channelDescription = reader[2].ToString() ?? "null",
                                channelType = reader[3].ToString() ?? "null"
                            };
                        }
                    }
                }

                conn.Close();
                return Ok(c);
            }

            return BadRequest("bad header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    [HttpGet(Path.GetChats)]
    public ActionResult<string> GetChats()
    {
        try
        {
            if (Request.Headers.TryGetValue("Username", out var uname) &&
                Request.Headers.TryGetValue("Password", out var psd))
            {
                MySqlConnection conn = new MySqlConnection(gb.dbconn);
                conn.Open();
                string id = new MySqlCommand(
                        $"SELECT `id` FROM `users` WHERE `userName` = '{uname}' AND `passwordHash` = '{psd}'", conn)
                    .ExecuteScalar()!.ToString()!;
                
                List<string> chatList = new List<string>();
                MySqlDataReader r =
                    new MySqlCommand($"SELECT `chatId` FROM `chatmembers` WHERE `userId` = '{id}'", conn)
                        .ExecuteReader();
                
                conn.Close();
                while (r.Read())
                {
                    chatList.Add(r[0].ToString()!);
                }
                return Ok(chatList.ToArray());
            }
            return BadRequest("Bad Header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
}