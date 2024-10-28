using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;
using Org.BouncyCastle.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainChatController)]
public class ChatController : Controller
{
    //Удаление пользователя из чата
    [HttpDelete(Path.DeleteFromChat)]
    public ActionResult DeleteFromChat(string CHAT_ID, string OTHER_USER_ID)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (Permissions.CheckForPermissions(Permission.CHAT_DELPEOPLE, username, password, CHAT_ID))
                {
                    using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `chatmembers` WHERE `userId` = @uid AND `chatId` = @cid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", OTHER_USER_ID);
                        cmd.Parameters.AddWithValue("@cid", CHAT_ID);
                    }

                    using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `permissions` WHERE `userId` = @uid AND `chatId` = @cid", conn)) 
                    {
                        cmd.Parameters.AddWithValue("@uid", OTHER_USER_ID);
                        cmd.Parameters.AddWithValue("@cid", CHAT_ID);
                    }

                    return Ok();
                }
                return Unauthorized("Фемка.");
            }
            return BadRequest("bad header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //Добавление пользователя в чат
    [HttpPatch(Path.AddToChat)]
    public ActionResult AddToChat(string CHAT_ID, string OTHER_USER_ID)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (Permissions.CheckForPermissions(Permission.CHAT_ADDPEOPLE, username, password, CHAT_ID))
                {
                    using(MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `chatmembers`(`chatId`, `userId`) VALUES (@cid,@uid)", c))
                    {
                        cmd.Parameters.AddWithValue("@cid", CHAT_ID);
                        cmd.Parameters.AddWithValue("@uid", OTHER_USER_ID);
                        
                        c.Open();
                        cmd.ExecuteNonQuery();
                        c.Close();
                        return Ok();
                    }
                }
                return Unauthorized("Фемка.");
            }
            return BadRequest("bad header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //Создание чата и авто-добавление создателя в пользователей чата
    [HttpPost(Path.CreateChat)]
    public ActionResult CreateChat(string CHAT_NAME, string CHAT_DESCRIPTION, string CHAT_TYPE)
    {
        try
        {
            if (CHAT_TYPE.ToLower() != "dm" && CHAT_TYPE.ToLower() != "voice" && CHAT_TYPE.ToLower() != "text")
            {
                return BadRequest("Неправильный вид канала");
            }
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (!Secure.TryLogin(username!, password!))
                {
                    return Unauthorized("Неверный логин\\пароль.");
                }

                if (CHAT_NAME == "" && CHAT_TYPE == "")
                {
                    return BadRequest("channelName или channelType не указаны.");
                }

                Chat c = new Chat();

                using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                using (MySqlCommand cmd =
                       new MySqlCommand(
                           "INSERT INTO `chats`(`channelName`, `channelDescription`, `type`) VALUES (@cn,@cd,@ct)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("@cn", CHAT_NAME);
                    cmd.Parameters.AddWithValue("@cd", CHAT_DESCRIPTION);
                    cmd.Parameters.AddWithValue("@ct", CHAT_TYPE);
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `chats` WHERE `channelName` = @cn", conn))
                {
                    cmd.Parameters.AddWithValue("@cn", CHAT_NAME);
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            c = new Chat
                            {
                                id = reader[0].ToString() ?? "null",
                                channelName = reader[1].ToString() ?? "null",
                                channelDescription = reader[2].ToString() ?? "null",
                                channelType = reader[3].ToString() ?? "null"
                            };
                        }
                    }
                    conn.Close();
                }

                User u = Secure.GetUser(username, password);
                
                using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                using (MySqlCommand cmd =
                       new MySqlCommand("INSERT INTO `chatmembers`( `chatId`, `userId`) VALUES (@cid,@uid)", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", c.id);
                    cmd.Parameters.AddWithValue("@uid", u.id);
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                
                using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                using (MySqlCommand cmd =
                       new MySqlCommand("INSERT INTO `permissions`(`userId`, `chatId`, `PermissionType`) VALUES (@uid,@cid,@permType)", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", c.id);
                    cmd.Parameters.AddWithValue("@uid", u.id);
                    cmd.Parameters.AddWithValue("@permType", CHAT_TYPE.ToUpper());
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return Ok(c);
            }

            return BadRequest("bad header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.TargetSite+ex.StackTrace+ex.InnerException+ex.HelpLink+ex.Source+ex.Message);
        }
    }
    
    //Получение всех чатов
    [HttpGet(Path.GetChats)]
    public ActionResult<string> GetChats()
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                List<string> chatList = new List<string>();
                using (MySqlConnection c = new MySqlConnection(gb.dbconn))
                using (MySqlCommand cmd = new MySqlCommand("SELECT `chatId` FROM `chatmembers` WHERE userId = @uid", c))
                {
                    cmd.Parameters.AddWithValue("@uid", Secure.GetUser(username, password).id);
                    c.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chatList.Add(reader[0].ToString());
                        }
                    }
                    c.Close();
            
                }
                List<Chat> channelList = new List<Chat>();
                foreach (string cid in chatList)
                {
                    using (MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `chats` WHERE `id` = @cid", c))
                    {
                        cmd.Parameters.AddWithValue("@cid", cid);
                        c.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                channelList.Add(new Chat
                                {
                                    id = reader[0].ToString(),
                                    channelName = reader[1].ToString(),
                                    channelDescription = reader[2].ToString(),
                                    channelType = reader[3].ToString()
                                });
                            }
                        }
                        c.Close();
                    }
                }
                return Ok(channelList.ToArray());
            }
            return BadRequest("Bad Header");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
}