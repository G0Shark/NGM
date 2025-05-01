using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class ChatsService
{
    public static bool IsChatExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM chats WHERE ID = @ID";

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
    
    public static void MakeChat(int SERVERID, string NAME, string DESCRIPTION, string TYPE, string ROLEID)
    {
        if (string.IsNullOrEmpty(ROLEID))
        {
            ROLEID = "0";
        }
        if (string.IsNullOrEmpty(NAME))
        {
            throw new NGMServiceExeption(new ConflictObjectResult("NAME не может быть пустым"));
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO chats (SERVERID, NAME, DESCRIPTION, TYPE, ROLEID) VALUES (@SERVERID, @NAME, @DESCRIPTION, @TYPE, @ROLEID)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@NAME", NAME);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);
                insertCommand.Parameters.AddWithValue("@TYPE", TYPE);
                insertCommand.Parameters.AddWithValue("@ROLEID", ROLEID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Чат создан"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void DeleteChat(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM chats WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Чат удалён"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void UpdateName(int ID, string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE chats SET NAME = @NAME WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("NAME Обнеовленно"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void UpdateDescription(int ID, string DESCRIPTION)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE chats SET DESCRIPTION = @DESCRIPTION WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("DESCRIPTION Обновленно"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void UpdateRoles(int ID, string ROLEID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE chats SET ROLEID = @ROLEID WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@ROLEID", ROLEID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("ROLEID Обновленно"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static Chat[] GetChats(int SERVERID)
    {
        List<Chat> chats = new List<Chat>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM chats WHERE SERVERID = @SERVERID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SERVERID", SERVERID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chats.Add(new Chat
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            TYPE = reader.GetString("TYPE"),
                            ROLEID = reader.GetString("ROLEID")
                        });
                    }
                }
            }
        }
        
        return chats.ToArray();
    }
    
    public static string GetNeededRolesForChat(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM chats WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString("ROLEID");
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Чата не существует"));
                    }
                }
            }
        }
    }
    
    public static Chat GetChat(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM chats WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Chat
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            TYPE = reader.GetString("TYPE"),
                            ROLEID = reader.GetString("ROLEID")
                        };
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Чата не существует"));
                    }
                }
            }
        }
    }
}

public class Chat
{
    public int ID { get; set; }
    public int SERVERID { get; set; }
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public string TYPE { get; set; }
    public string ROLEID { get; set; }
}