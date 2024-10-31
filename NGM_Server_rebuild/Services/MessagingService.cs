using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class MessagingService
{
    //Отправка сообщения
    public static void SendMessage(Message message)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.MessagingServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("INSERT INTO `messages`(`chatId`, `senderId`,`content`, `answeringMessageId`, `isDM`) VALUES (@chatid,@senderid,@content,@answerid,@isDM)", connection))
            {
                command.Parameters.AddWithValue("@chatid", message.chatId);
                command.Parameters.AddWithValue("@senderid", message.senderId);
                command.Parameters.AddWithValue("@content", message.content);
                command.Parameters.AddWithValue("@answerid", message.answeringMessageId);
                command.Parameters.AddWithValue("@isDM", message.isDM);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Удаление сообщения
    public static void DeleteMessage(Message message)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.MessagingServiceConnectionString))
        {
            connection.Open();

            //Проверка на наличие сообщения
            bool isMessageExist = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM messages WHERE `messageId` = @id", connection))
            {
                command.Parameters.AddWithValue("@u", message.id);

                isMessageExist = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isMessageExist)
            {
                throw new NGMServiceExeption(new BadRequestObjectResult("Сообщения не найдено"));
            }
            
            using (MySqlCommand command = new MySqlCommand("DELETE FROM `messages` WHERE `messageId`=@messageid", connection))
            {
                command.Parameters.AddWithValue("@messageid", message.id);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Изменение сообщения
    public static void ModifyMessage(Message message)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.MessagingServiceConnectionString))
        {
            connection.Open();

            //Проверка на наличие сообщения
            bool isMessageExist = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM messages WHERE `messageId` = @id", connection))
            {
                command.Parameters.AddWithValue("@u", message.id);

                isMessageExist = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isMessageExist)
            {
                throw new NGMServiceExeption(new BadRequestObjectResult("Сообщения не найдено"));
            }
            
            using (MySqlCommand command = new MySqlCommand("UPDATE `messages` SET `content`=@content WHERE `messageId` = @messageid", connection))
            {
                command.Parameters.AddWithValue("@messageid", message.id);
                command.Parameters.AddWithValue("@content", message.content);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Получить все сообщения чата
    public static Message[] GetMessages(string chatid, string isDM)
    {
        List<Message> messages = new List<Message>();

        using (MySqlConnection connection = new MySqlConnection(gb.MessagingServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `messages` WHERE `chatId`=@cid AND `isDM`=@dm", connection))
            {
                command.Parameters.AddWithValue("@cid", chatid);
                command.Parameters.AddWithValue("@dm", isDM);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            id = reader[0].ToString()??"null",
                            chatId = reader[1].ToString()??"null",
                            senderId = reader[2].ToString()??"null",
                            time = reader[3].ToString()??"null",
                            content = reader[4].ToString()??"null",
                            answeringMessageId = reader[5].ToString()??"null",
                            isDM = (reader[6].ToString()??"null") == "1"
                        });
                    }
                }
            }
        }
        
        return messages.ToArray();
    }
}