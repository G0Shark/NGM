using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class ServerService
{
    //Создание сервера
    public static void CreateServer(Server server)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.ServerServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("INSERT INTO `server`(`ownerId`, `name`, `avatarUrl`) VALUES ('@ownerid','@name','@avatar')", connection))
            {
                command.Parameters.AddWithValue("@ownerid", server.ownerId);
                command.Parameters.AddWithValue("@name", server.Name);
                command.Parameters.AddWithValue("@avatar", server.avatar);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Удалить сервер
    public static void DeleteServer(Server server)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.ServerServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("DELETE FROM `server` WHERE `serverId`=@serverid", connection))
            {
                command.Parameters.AddWithValue("@serverid", server.serverId);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Изменить владельца
    public static void UpdateServerOwner(Server server, string newOwnerId)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.ServerServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("UPDATE `server` SET `ownerId`=@ownerid WHERE `serverId` = @serverid", connection))
            {
                command.Parameters.AddWithValue("@serverid", server.serverId);
                command.Parameters.AddWithValue("@ownerid", newOwnerId);
            }
            
            connection.Close();
        }
    }
    
    //Изменить название
    public static void UpdateServerName(Server server, string newName)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.ServerServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("UPDATE `server` SET `name`=@newname WHERE `serverId` = @serverid", connection))
            {
                command.Parameters.AddWithValue("@serverid", server.serverId);
                command.Parameters.AddWithValue("@newname", newName);
            }
            
            connection.Close();
        }
    }
}