using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class RoleService
{
    //Создание роли
    public static void CreateRole(Role role)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RoleServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("INSERT INTO `roles`(`serverId`, `name`) VALUES (@sid,@name)", connection))
            {
                command.Parameters.AddWithValue("@sid", role.serverId);
                command.Parameters.AddWithValue("@name", role.name);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Удаление роли
    public static void DeleteRole(Role role)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.RoleServiceConnectionString))
        {
            connection.Open();

            //Проверка на наличие роли
            bool isRoleExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `roles` WHERE roleId = @rid", connection))
            {
                command.Parameters.AddWithValue("@rid", role.id);
                isRoleExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isRoleExists)
            {
                throw new NGMServiceExeption(new NotFoundObjectResult("Роли не существует"));
            }
            
            //Удаление роли
            using (MySqlCommand command = new MySqlCommand("DELETE FROM `roles` WHERE `roleId`=@rid", connection))
            {
                command.Parameters.AddWithValue("@rid", role.id);
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Модификация роли (только название)
    public static void UpdateRole(Role role)
    {
        if (role.name == "")
        {
            throw new NGMServiceExeption(new ConflictObjectResult("Изменение роли без изменений бесмысленно"));
        }

        using (MySqlConnection connection = new MySqlConnection(gb.RoleServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("UPDATE `roles` SET `name`=@name WHERE `roleId` = @id", connection))
            {
                command.Parameters.AddWithValue("@id", role.id);
                command.Parameters.AddWithValue("@name", role.name);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
}