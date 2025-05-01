using System.Security;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class ServersService
{
    public static bool TryJoinWithPassword(int ID, string JOINPASSWORD)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT VISIBLE FROM servers WHERE ID=@ID AND JOINPASSWORD=@JOINPASSWORD";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", JOINPASSWORD);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
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
    public static bool IsVisible(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT VISIBLE FROM servers WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetBoolean(0);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    public static bool IsServerExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
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
    
    public static bool IsServerExists(string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE NAME=@NAME";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
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
    
    public static Server GetServer(string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE NAME=@NAME";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Server
                        {
                            ID = reader.GetInt32("ID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            JOINPASSWORD = reader.GetString("JOINPASSWORD"),
                            VISIBLE = reader.GetBoolean("VISIBLE"),
                            OWNERID = reader.GetInt32("OWNERID"),
                            CREATION_DATE = reader.GetDateTime("CREATION_DATE"),
                        };
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не существует"));
                    }
                }
            }
        }
    }
    
    public static bool IsOwner(int ID, int OWNERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE ID=@ID AND OWNERID=@OWNERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@OWNERID", OWNERID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
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
    
    public static void Make(string NAME, string? DESCRIPTION, string JOINPASSWORD, bool VISIBLE, int OWNERID)
    {
        if (string.IsNullOrEmpty(DESCRIPTION))
        {
            DESCRIPTION = "Нет описания, ну думаю что это крутой сервер!";
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO servers (NAME, DESCRIPTION, JOINPASSWORD, VISIBLE, OWNERID) VALUES (@NAME, @DESCRIPTION, @JOINPASSWORD, @VISIBLE, @OWNERID)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", JOINPASSWORD);
                insertCommand.Parameters.AddWithValue("@VISIBLE", VISIBLE);
                insertCommand.Parameters.AddWithValue("@OWNERID", OWNERID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }

    public static void ChangeName(int ID, string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET NAME=@NAME WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void ChangeDescription(int ID, string DESCRIPTION)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET DESCRIPTION=@DESCRIPTION WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void ChangeOwner(int ID, int OWNERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET OWNERID=@OWNERID WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@OWNERID", OWNERID);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET JOINPASSWORD=@JOINPASSWORD WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", JOINPASSWORD);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void ChangeVisible(int ID, bool VISIBLE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET VISIBLE=@VISIBLE WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@VISIBLE", VISIBLE);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public static void Delete(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM servers WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
}

public class Server
{
    public int ID { get; set; }
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public string JOINPASSWORD  { get; set; }
    public bool VISIBLE { get; set; }
    public int OWNERID { get; set; }
    public DateTime CREATION_DATE { get; set; }
}