using System.Security;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class ServersService
{
    
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