using System.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class ChatService
{
    //Создание чата
    public static void CreateTextChat(Chat chat)
    {
        //Проверка входного сообщения
        if (chat.channelName == "")
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Имя канала не может быть пустым"));
        }
        
        //Создание чата
        using (MySqlConnection connection = new MySqlConnection(gb.ChatServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("INSERT INTO `chats`(`channelName`, `channelDescription`, `type`, `serverId`) VALUES (@cn,@cd,@t,@sid)", connection))
            {
                command.Parameters.AddWithValue("@cn", chat.channelName);
                command.Parameters.AddWithValue("@cd", chat.channelDescription);
                command.Parameters.AddWithValue("@t", chat.channelType);
                command.Parameters.AddWithValue("@sid", chat.serverId);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Изменение чата
    public static void ModifyChat(Chat chat)
    {
        //Проверка чата на модификацию, все пустые строки обозначают то что это не именяем
        bool modifName = true;
        bool modifDesc = true;
        bool modifType = true;

        if (chat.channelName == "")
        {
            modifName = false;
        }
        if (chat.channelDescription == "")
        {
            modifDesc = false;
        }
        if (chat.channelType == "")
        {
            modifType = false;
        }
        if (chat.channelType != "TEXT" || chat.channelType != "VOICE")
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Неверный тип чата"));
        }
        
        //Изменение
        using (MySqlConnection connection = new MySqlConnection(gb.ChatServiceConnectionString))
        {
            connection.Open();

            //Проверка на наличие чата
            bool isChatExist = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM chats WHERE `id` = @id", connection))
            {
                command.Parameters.AddWithValue("@id", chat.id);

                isChatExist = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isChatExist)
            {
                throw new NGMServiceExeption(new NotFoundObjectResult("Чата не существует"));
            }
            
            if (modifName)
            {
                using (MySqlCommand command = new MySqlCommand("UPDATE `chats` SET `channelName`=@p WHERE `serverId` = @sid", connection))
                {
                    command.Parameters.AddWithValue("@p", chat.channelName);
                    command.Parameters.AddWithValue("@sid", chat.serverId);
                }
            }
            
            if (modifDesc)
            {
                using (MySqlCommand command = new MySqlCommand("UPDATE `chats` SET `channelDescription`=@p WHERE `serverId` = @sid", connection))
                {
                    command.Parameters.AddWithValue("@p", chat.channelDescription);
                    command.Parameters.AddWithValue("@sid", chat.serverId);
                }
            }
            
            if (modifType)
            {
                using (MySqlCommand command = new MySqlCommand("UPDATE `chats` SET `type`=@p WHERE `serverId` = @sid", connection))
                {
                    command.Parameters.AddWithValue("@p", chat.channelType);
                    command.Parameters.AddWithValue("@sid", chat.serverId);
                }
            }
        }
    }
    
    //Удаление чата
    public static void DeleteChat(Chat chat)
    {
        //Проверка на наличие чата
        bool isChatExist = false;
        using (MySqlConnection connection = new MySqlConnection(gb.ChatServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM chats WHERE `id` = @id", connection))
            {
                command.Parameters.AddWithValue("@id", chat.id);

                isChatExist = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isChatExist)
            {
                connection.Close();
                throw new NGMServiceExeption(new BadRequestObjectResult("Чата не существует"));
            }

            using (MySqlCommand command = new MySqlCommand("DELETE FROM `chats` WHERE `id` = @id", connection))
            {
                command.Parameters.AddWithValue("@id", chat.id);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Получить все чаты сервера
    public static Chat[] GetAllChats(string serverId)
    {
        List<Chat> chats = new List<Chat>();

        using (MySqlConnection connection = new MySqlConnection(gb.ChatServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `chats` WHERE `serverId`=@sid", connection))
            {
                command.Parameters.AddWithValue("@sid", serverId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chats.Add(new Chat
                        {
                            id = reader[0].ToString()??"null",
                            channelName = reader[1].ToString()??"null",
                            channelDescription = reader[2].ToString()??"null",
                            channelType = reader[3].ToString()??"null",
                            serverId = reader[4].ToString()??"null",
                        });
                    }
                }
            }
        }
        
        return chats.ToArray();
    }
}