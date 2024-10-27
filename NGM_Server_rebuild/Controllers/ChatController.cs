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
    public ActionResult DeleteFromChat(string chatId, string userId)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (Permissions.CheckForPermissions(Permission.CHAT_DELPEOPLE, username, password, chatId))
                {
                    using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `chatmembers` WHERE `userId` = @uid AND `chatId` = @cid", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@cid", chatId);
                    }

                    using (MySqlConnection conn = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM `permissions` WHERE `userId` = @uid AND `chatId` = @cid", conn)) 
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@cid", chatId);
                    }

                    return Ok();
                }
                return Unauthorized("Нет прав");
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
    public ActionResult AddToChat(string chatid, string other_id)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (Permissions.CheckForPermissions(Permission.CHAT_ADDPEOPLE, username, password, chatid))
                {
                    using(MySqlConnection c = new MySqlConnection(gb.dbconn))
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `chatmembers`(`chatId`, `userId`) VALUES (@cid,@uid)", c))
                    {
                        cmd.Parameters.AddWithValue("@cid", chatid);
                        cmd.Parameters.AddWithValue("@uid", other_id);
                        
                        cmd.ExecuteNonQuery();
                        return Ok();
                    }
                }
                return Unauthorized("Нет прав");
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
    public ActionResult CreateChat(string channelName, string channelDescription, string channelType)
    {
        try
        {
            if (channelType.ToLower() != "dm" || channelType.ToLower() != "voice" || channelType.ToLower() != "text")
            {
                return BadRequest("Неправильный вид канала");
            }
            if (Secure.IsValidHeader(Request, out string username, out string password))
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

                User u = Secure.GetUser(username, password);
                
                using (MySqlCommand cmd =
                       new MySqlCommand("INSERT INTO `chatmembers`( `chatId`, `userId`) VALUES (@cid,@uid)", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", c.Id);
                    cmd.Parameters.AddWithValue("@uid", u.id);
                    
                    cmd.ExecuteNonQuery();
                }
                
                using (MySqlCommand cmd =
                       new MySqlCommand("INSERT INTO `permissions`(`userId`, `chatId`, `PermissionType`) VALUES (@uid,@cid,@permType)", conn))
                {
                    cmd.Parameters.AddWithValue("@cid", c.Id);
                    cmd.Parameters.AddWithValue("@uid", u.id);
                    cmd.Parameters.AddWithValue("@permType", channelType.ToUpper());
                    
                    cmd.ExecuteNonQuery();
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
                List<Channel> channelList = new List<Channel>();
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
                                channelList.Add(new Channel
                                {
                                    Id = reader[0].ToString(),
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