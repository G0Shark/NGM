using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;

namespace NGMapi.Infrastructure.Repositories;

public class MessagingRepository : IMessagingRepository
{
    public bool IsUserAreSender(int ID, int SENDERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM messages WHERE ID=@ID AND SENDERID = @SENDERID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    
    public bool IsMessageExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM messages WHERE ID=@ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    
    public Message SendMessage(Message message)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO messages (CHATID, SENDERID, MESSAGE, REPLYFORID, DMCHAT) VALUES (@CHATID, @SENDERID, @MESSAGE, @REPLYFORID, @DMCHAT); SELECT * FROM messages WHERE ID=LAST_INSERT_ID();";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@CHATID", message.CHATID);
                insertCommand.Parameters.AddWithValue("@SENDERID", message.SENDERID);
                insertCommand.Parameters.AddWithValue("@MESSAGE", message.MESSAGE);
                insertCommand.Parameters.AddWithValue("@REPLYFORID", message.REPLYFORID);
                insertCommand.Parameters.AddWithValue("@DMCHAT", message.DMCHAT);

                var reader = insertCommand.ExecuteReader();
                if (reader.Read())
                {
                    return new Message
                    {
                        ID = reader.GetInt32("ID"),
                        CHATID = reader.GetInt32("CHATID"),
                        SENDERID = reader.GetInt32("SENDERID"),
                        DMCHAT = reader.GetBoolean("DMCHAT"),
                        MESSAGE = reader.GetString("MESSAGE"),
                        REPLYFORID = reader.GetInt32("REPLYFORID"),
                        TIMESTAMP = reader.GetDateTime("TIMESTAMP"),
                    };
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void EditMessage(int ID, string NEWCONTENT)
    {
        if (!IsMessageExists(ID))
        {
            throw new NGMServiceExeption(new NotFoundObjectResult("Сообщения не существует"));
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE messages SET MESSAGE=@NEWCONTENT WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NEWCONTENT", NEWCONTENT);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Сообщение измененно"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void DeleteMessage(int ID)
    {
        if (!IsMessageExists(ID))
        {
            throw new NGMServiceExeption(new NotFoundObjectResult("Сообщения не существует"));
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM messages WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Сообщение измененно"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }

    public Message[] GetMessages(int CHATID, bool DMCHAT)
    {
        List<Message> messages = new List<Message>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM messages WHERE CHATID = @CHATID AND DMCHAT = @DMCHAT";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@CHATID", CHATID);
                checkCommand.Parameters.AddWithValue("@DMCHAT", DMCHAT);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            ID = reader.GetInt32("ID"),
                            CHATID = reader.GetInt32("CHATID"),
                            SENDERID = reader.GetInt32("SENDERID"),
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                            TIMESTAMP = reader.GetDateTime("TIMESTAMP"),
                        });
                    }
                }
            }
        }
        
        return messages.ToArray();
    }
    
    public Message[] GetMessagesFromDM(int CHATID1, int CHATID2)
    {
        List<Message> messages = new List<Message>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM messages WHERE CHATID = @CHATID1 AND DMCHAT = 1 OR CHATID = @CHATID2 AND DMCHAT = 1";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@CHATID1", CHATID1);
                checkCommand.Parameters.AddWithValue("@CHATID2", CHATID2);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new Message
                        {
                            ID = reader.GetInt32("ID"),
                            CHATID = reader.GetInt32("CHATID"),
                            SENDERID = reader.GetInt32("SENDERID"),
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                            TIMESTAMP = reader.GetDateTime("TIMESTAMP"),
                        });
                    }
                }
            }
        }
        
        return messages.ToArray();
    }
    
    public Message GetMessage(int ID)
    {
        List<Message> messages = new List<Message>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM messages WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Message
                        {
                            ID = reader.GetInt32("ID"),
                            CHATID = reader.GetInt32("CHATID"),
                            SENDERID = reader.GetInt32("SENDERID"),
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                            TIMESTAMP = reader.GetDateTime("TIMESTAMP"),
                        };
                    }
                }
            }
        }

        throw new NGMServiceExeption(new ConflictObjectResult("Сообщение не найдено"));
    }
}