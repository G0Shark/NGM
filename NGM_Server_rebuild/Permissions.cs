using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild;

public static class Permissions
{
    public static bool CheckForPermissions(Permission[] perms, string username, string password, string chatid)
    {
        if (!Secure.TryLogin(username, password))
        {
            throw new Exception("Неверный логин\\пароль");
        }

        Permission[] hasPerms = GetPermissions(username, password, chatid);
        if (hasPerms.Contains(Permission.ALL))
        {
            return true;
        }

        foreach (Permission p in perms)
        {
            if (!hasPerms.Contains(p))
            {
                return false;
            }
        }
        return true;
    }
    public static bool CheckForPermissions(Permission perms, string username, string password, string chatid)
    {
        if (!Secure.TryLogin(username, password))
        {
            throw new Exception("Неверный логин\\пароль");
        }

        Permission[] hasPerms = GetPermissions(username, password, chatid);
        if (hasPerms.Contains(Permission.ALL) || hasPerms.Contains(perms))
        {
            return true;
        }
        return true;
    }
    public static Permission[] GetPermissions(string username, string password, string chatid)
    {
        if (!Secure.TryLogin(username, password)) { throw new Exception("Неверный Логин\\Пароль"); }
        List<Permission> p = new List<Permission>();
        using (MySqlConnection c = new MySqlConnection(gb.dbconn))
        using (MySqlCommand cmd =
               new MySqlCommand("SELECT `PermissionType` FROM `permissions` WHERE `userId` = @uid AND `chatId` = @cid",
                   c))
        {
            cmd.Parameters.AddWithValue("@uid", username);
            cmd.Parameters.AddWithValue("@cid", chatid);
            
            foreach (char ch in cmd.ExecuteScalar().ToString())
            {
                switch (ch)
                {
                    case 'a': p.Add(Permission.ALL); break;
                    case 'd': p.Add(Permission.DELETE); break;
                    case 'c': p.Add(Permission.CHANGE); break;
                    case 'n': p.Add(Permission.CHAT_NAME); break;
                    case 'p': p.Add(Permission.CHAT_NEEDED_PERMS); break;
                    case 's': p.Add(Permission.CHAT_DESC); break;
                    case 'u': p.Add(Permission.CHAT_ADDPEOPLE); break;
                    case 'e': p.Add(Permission.CHAT_DELPEOPLE); break;
                    case 'm': p.Add(Permission.DELETING_MSG); break;
                    case 'v': p.Add(Permission.VOICE_NAME); break;
                    case 'w': p.Add(Permission.VOICE_DESC); break;
                    case 'j': p.Add(Permission.VOICE_MUTE); break;
                    case 'k': p.Add(Permission.VOICE_KICK); break;
                }
            }
        }

        return p.ToArray();
    }
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