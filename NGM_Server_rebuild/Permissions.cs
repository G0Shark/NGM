using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild;

public static class Permissions
{
    public static bool IsChatExist(string chatid)
    {
        MySqlConnection c = new MySqlConnection(gb.dbconn);
        c.Open();
        using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM `chats` WHERE `id` = @cid", c))
        {
            cmd.Parameters.AddWithValue("@cid", chatid);
            
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            c.Close();
                
            return count > 0;
        }
    }
    
    public static bool IsUserInChat(string username, string password, string chatid)
    {
        MySqlConnection c = new MySqlConnection(gb.dbconn);
        if (!Secure.TryLogin(username, password)) { throw new Exception("Неверный Логин\\Пароль"); }
        User u = Secure.GetUser(username, password);
        c.Open();
        using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM `chatmembers` WHERE `userId` = @uid AND `chatId` = @cid", c))
        {
            cmd.Parameters.AddWithValue("@uid", u.id);
            cmd.Parameters.AddWithValue("@cid", chatid);
            
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            c.Close();
                
            return count > 0;
        }
    }
}