using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class MessagingService
{
    public static bool IsUserAreSender(int ID, int SENDERID)
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
    
    public static bool IsMessageExists(int ID)
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
    
    public static void SendMessage(int CHATID, int SENDERID, string MESSAGE, int REPLYFORID, bool DMCHAT)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO messages (CHATID, SENDERID, MESSAGE, REPLYFORID, DMCHAT) VALUES (@CHATID, @SENDERID, @MESSAGE, @REPLYFORID, @DMCHAT)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@CHATID", CHATID);
                insertCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                insertCommand.Parameters.AddWithValue("@MESSAGE", MESSAGE);
                insertCommand.Parameters.AddWithValue("@REPLYFORID", REPLYFORID);
                insertCommand.Parameters.AddWithValue("@DMCHAT", DMCHAT);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Сообщение отправлено"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void EditMessage(int ID, string NEWCONTENT)
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
    
    public static void DeleteMessage(int ID)
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

    public static Message[] GetMessages(int CHATID, bool DMCHAT)
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
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                        });
                    }
                }
            }
        }
        
        return messages.ToArray();
    }
    
    public static Message[] GetMessagesFromDM(int CHATID1, int CHATID2)
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
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                        });
                    }
                }
            }
        }
        
        return messages.ToArray();
    }
    
    public static Message GetMessage(int ID)
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
                            DMCHAT = reader.GetBoolean("DMCHAT"),
                            MESSAGE = reader.GetString("MESSAGE"),
                            REPLYFORID = reader.GetInt32("REPLYFORID"),
                        };
                    }
                }
            }
        }

        throw new NGMServiceExeption(new ConflictObjectResult("Сообщение не найдено"));
    }
}

public class Message
{
    public int ID { get; set; }
    public int CHATID { get; set; }
    public string MESSAGE { get; set; }
    public int REPLYFORID { get; set; }
    public Timestamp TIMESTAMP { get; set; }
    public bool DMCHAT { get; set; }
}