using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class RolePermissionService
{
    //Установление данных возможностей в данном чате
    public static void SetRolePermissions(RolePermission rolePermission, Chat[] chats)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RolePermissionServiceConnectionString))
        {
            connection.Open();

            foreach (Chat chat in chats)
            {
                //Проверка на наличие чата
                bool isChatExists = false;
                using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `roleperms` WHERE `roleid` = @rid AND `chatid`=@cid", connection))
                {
                    command.Parameters.AddWithValue("@rid", rolePermission.RoleId);
                    command.Parameters.AddWithValue("@cid", chat.id);
                    
                    isChatExists = (int)command.ExecuteScalar() > 0;
                }
                if (!isChatExists)
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Чата нет в списке"));
                }
                
                using (MySqlCommand command = new MySqlCommand("UPDATE `roleperms` SET `permisions`=@pstr WHERE `chatid`=@cid", connection))
                {
                    command.Parameters.AddWithValue("@pstr", rolePermission.PermissionsStr);
                    command.Parameters.AddWithValue("@cid", chat.id);
                    
                    command.ExecuteNonQuery();
                }
            }
            
            connection.Close();
        }
    }
    
    //Сделать все права чатов 
    public static void MakeOrModifyRolePermissionChats(RolePermission rolePermission, Chat[] chats)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RolePermissionServiceConnectionString))
        {
            connection.Open();

            foreach (Chat chat in chats)
            {
                //Проверка на наличие чата
                bool isChatExists = false;
                using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `roleperms` WHERE `roleid` = @rid AND `chatid`=@cid", connection))
                {
                    command.Parameters.AddWithValue("@rid", rolePermission.RoleId);
                    command.Parameters.AddWithValue("@cid", chat.id);
                    
                    isChatExists = (int)command.ExecuteScalar() > 0;
                }
                if (!isChatExists)
                {
                    using (MySqlCommand command2 = new MySqlCommand("INSERT INTO `roleperms`(`roleid`, `chatid`, `permisions`) VALUES (@rid,@cid,@permstr)", connection))
                    {
                        command2.Parameters.AddWithValue("@rid", rolePermission.RoleId);
                        command2.Parameters.AddWithValue("@cid", chat.id);
                        command2.Parameters.AddWithValue("@permstr", rolePermission.PermissionsStr);
                        
                        command2.ExecuteNonQuery();
                    }
                }
            }
        }
    }
    
    //Удалить все упомянания этой роли
    public static void RemoveRoleRermissions(RolePermission rolePermission)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RolePermissionServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("DELETE FROM `roleperms` WHERE `roleid` = @rid", connection))
            {
                command.Parameters.AddWithValue("@rid", rolePermission.RoleId);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Промодифицировать права для отпределённого чата
    public static void ModifOnlyOneChat(RolePermission rolePermission)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RolePermissionServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("UPDATE `roleperms` SET `permisions`='[value-4]' WHERE `roleid`=@rid AND `chatid`=@cid", connection))
            {
                command.Parameters.AddWithValue("@rid", rolePermission.RoleId);
                command.Parameters.AddWithValue("@cid", rolePermission.ChatId);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
}