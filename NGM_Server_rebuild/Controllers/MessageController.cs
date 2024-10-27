using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainMessageController)]
public class MessageController : Controller
{
    [HttpDelete(Path.DeleteMessage)]
    public ActionResult DeleteMessage(string msg_id)
    {
        try
        {
            if (Request.Headers.TryGetValue("Username", out var username) &&
                Request.Headers.TryGetValue("Password", out var password))
            {
                if (!Secure.TryLogin(username, password)) { return Unauthorized("Логин\\Пароль неверны"); }
                
            }
            return BadRequest("No AUTH_HEADER");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //Получить все сообщения с чата
    [HttpGet(Path.GetMessages)]
    public ActionResult<List<Message>> GetMessages(string chatId)
    {
        try
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
                            id = reader[0].ToString() ?? "null",
                            chatId = reader[1].ToString() ?? "null",
                            senderId = reader[2].ToString() ?? "null",
                            time = reader[3].ToString() ?? "null",
                            content = reader[4].ToString() ?? "null",
                            answeringMessageId = reader[5].ToString() ?? "null"
                        });
                    }
                }
            }

            conn.Close();
            return messages;
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
}