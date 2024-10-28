using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainMessageController)]
public class MessageController : Controller
{
    //Модифицировать сообщение
    [HttpPatch(Path.ModifyMessage)]
    public ActionResult ModifyMessage(string MSG_ID, string NEW_CONTENT)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (IsOwnMessage(MSG_ID, Secure.GetUser(username, password).id))
                {
                    using (MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE `messages` SET `content`= @content WHERE `messageId` = @mid", c))
                    {
                        cmd.Parameters.AddWithValue("@content", NEW_CONTENT);
                        cmd.Parameters.AddWithValue("@mid", MSG_ID);
                        
                        c.Open();
                        cmd.ExecuteNonQuery();
                        c.Close();
                        return Ok();
                    }
                }

                if (Permissions.CheckForPermissions(Permission.CHANGE, username, password, GetChatIdByMessage(MSG_ID)))
                {
                    using (MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE `messages` SET `content`= @content WHERE `messageId` = @mid", c))
                    {
                        cmd.Parameters.AddWithValue("@content", NEW_CONTENT);
                        cmd.Parameters.AddWithValue("@mid", MSG_ID);
                        
                        c.Open();
                        cmd.ExecuteNonQuery();
                        c.Close();
                        return Ok();
                    }
                }
                
                return Unauthorized("Фемка.");
            }
            return BadRequest("BAD_HEADER");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //Удаление сообщений
    [HttpDelete(Path.DeleteMessage)]
    public ActionResult DeleteMessage(string MSG_ID)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (!Secure.TryLogin(username, password)) { return Unauthorized("Логин\\Пароль неверны"); }

                if (Permissions.CheckForPermissions(Permission.DELETE, username, password, GetChatIdByMessage(MSG_ID)))
                {
                    using (MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `messages` WHERE `messageId` = @mid", c))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@mid", MSG_ID));
                        c.Open();
                        cmd.ExecuteNonQuery();
                        c.Close();
                        return Ok();
                    }
                }
                return Unauthorized("Фемка.");
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
    public ActionResult<List<Message>> GetMessages(string CHAT_ID)
    {
        try
        {
            List<Message> messages = new List<Message>();
            MySqlConnection conn = new MySqlConnection(gb.dbconn);
            conn.Open();
            if (Request.Headers.TryGetValue("Username", out var uname) &&
                Request.Headers.TryGetValue("Password", out var psd))
            {
                if (!Permissions.IsUserInChat(uname, psd, CHAT_ID))
                {
                    return Unauthorized("Фемка.");
                }
            }
            else
            {
                return BadRequest("BAD_HEADER");
            }

            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `messages` WHERE `chatId` = @cid", conn))
            {
                cmd.Parameters.AddWithValue("@cid", CHAT_ID);
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
    
    //ЛОКАЛЬНЫЕ ФУНКЦИИ -------------------------
    
    //Локальная полезная функция для получения чата по айдишнику сообщения
    private string GetChatIdByMessage(string msg_id)
    {
        using (MySqlConnection c = new MySqlConnection(gb.dbconn))
        using (MySqlCommand cmd = new MySqlCommand("SELECT `chatId` FROM `messages` WHERE `messageId` = @mid", c))
        {
            cmd.Parameters.AddWithValue("@mid", msg_id);
            
            return cmd.ExecuteScalar().ToString();
        }
    }
    
    //Локальная функция для быстрого узнавания твоё ли это сообщение
    private bool IsOwnMessage(string msg_id, string user_id)
    {
        using (MySqlConnection c = new MySqlConnection(gb.dbconn))
        using (MySqlCommand cmd = new MySqlCommand("SELECT `senderId` FROM `messages` WHERE `messageId` = @mid", c))
        {
            cmd.Parameters.AddWithValue("@mid", msg_id);
            string senderId = cmd.ExecuteScalar().ToString();
            
            return senderId == user_id;
        }
    }
}