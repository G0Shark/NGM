using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;
using Org.BouncyCastle.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : Controller
{
    /*[HttpPatch("addtochat")]
    public ActionResult AddToChat(string username, string password, string chatid, string other_username)
    {
        
    }*/
    [HttpPost("create")]
    public ActionResult CreateChat(string channelName, string channelDescription, string channelType)
    {
        if (Request.Headers.TryGetValue("Username", out var username) &&
            Request.Headers.TryGetValue("Password", out var password))
        {
            if (!Secure.TryLogin(username, password)) { return BadRequest("Неверный логин\\пароль."); }
            if (channelName == "" && channelType == "") { return BadRequest("channelName или channelType не указаны."); }

            Channel c = new Channel();
            
            Console.WriteLine("Sigmasigmasigma"); //Это тествой коммент который я пишу очень быстро
            
            MySqlConnection conn = new MySqlConnection(gb.dbconn);
            conn.Open();
            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `chats`(`channelName`, `channelDescription`, `type`) VALUES (@cn,@cd,@ct)", conn))
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
                            Id = reader[0].ToString(),
                            channelName = reader[1].ToString(),
                            channelDescription = reader[2].ToString(),
                            channelType = reader[3].ToString()
                        };
                    }
                }
            }
            conn.Close();
            return Ok(c);
        }
        else
        {
            return BadRequest("bad header");
        }
    }
    
    [HttpGet("messages")]
    public ActionResult<List<Message>> GetMessages(string chatId)
    {
        List<Message> messages = new List<Message>();
        MySqlConnection conn = new MySqlConnection(gb.dbconn);
        conn.Open();
        if (Request.Headers.TryGetValue("Username", out var uname) &&
            Request.Headers.TryGetValue("Password", out var psd))
        {
            if (!Permissions.IsUserInChat(uname, psd, chatId))
            {
                return BadRequest("Not Authorized");
            }
        }
        else
        {
            return BadRequest("Username and/or password are required in header");
        }
        using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `messages` WHERE `chatId` = @cid", conn))
        {
            cmd.Parameters.AddWithValue("@cid", chatId);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(new Message
                    {
                        id = reader[0].ToString(),
                        chatId = reader[1].ToString(),
                        senderId = reader[2].ToString(),
                        time = reader[3].ToString(),
                        content = reader[4].ToString(),
                        answeringMessageId = reader[5].ToString()
                    });
                }
            }
        }
        conn.Close();
        return messages;
    }
    
    [HttpGet("chats")]
    public ActionResult<string> GetChats()
    {
        MySqlConnection conn = new MySqlConnection(gb.dbconn);
        conn.Open();

        string id = "";
        if (Request.Headers.TryGetValue("Username", out var uname) && Request.Headers.TryGetValue("Password", out var psd))
        {
            id = new MySqlCommand($"SELECT `id` FROM `users` WHERE `userName` = '{uname}' AND `passwordHash` = '{psd}'", conn).ExecuteScalar().ToString();
        }
        else
        {
            conn.Close();
            return BadRequest("Bad Header");
        }
        
        List<string> chatList = new List<string>();
        MySqlDataReader r = new MySqlCommand($"SELECT `chatId` FROM `chatmembers` WHERE `userId` = '{id}'", conn)
            .ExecuteReader();
        while (r.Read())
        {
            chatList.Add(r[0].ToString());
        }
        
        return Ok(chatList.ToArray());
    }
}