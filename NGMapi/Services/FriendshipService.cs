using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class FriendshipService
{
    public static bool IsFriendshipExists(int SENDERID, int ACCEPTERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE SENDERID = @SENDERID AND ACCEPTERID = @ACCEPTERID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                checkCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);
                
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
    
    public static bool IsFriendshipExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE ID = @ID";

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
    
    public static bool IsFriendRequest(int SENDERID, int ACCEPTERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE SENDERID = @SENDERID AND ACCEPTERID = @ACCEPTERID AND FRENDSHIPSTATE = 2";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                checkCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);
                
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
    
    public static bool IsFriend(int SENDERID, int ACCEPTERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE SENDERID = @SENDERID AND ACCEPTERID = @ACCEPTERID AND FRENDSHIPSTATE = 3";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                checkCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);
                
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
    
    public static bool IsBlocked(int SENDERID, int ACCEPTERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE SENDERID = @SENDERID AND ACCEPTERID = @ACCEPTERID AND FRENDSHIPSTATE = 4";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                checkCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);
                
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
    
    public static void FrendshipChange(int SENDERID, int ACCEPTERID, int STATE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE friendships SET FRENDSHIPSTATE=@STATE WHERE SENDERID=@SENDERID AND ACCEPTERID=@ACCEPTERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@STATE", STATE);
                insertCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                insertCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);

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

    public static void MakeFriendship(int SENDERID, int ACCEPTERID, int STATE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO friendships (SENDERID, ACCEPTERID, FRENDSHIPSTATE) VALUES (@SENDERID, @ACCEPTERID, @STATE)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                insertCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);
                insertCommand.Parameters.AddWithValue("@STATE", STATE);

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