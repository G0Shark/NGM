using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Domain.Entities;

namespace NGM_Server_rebuild.Services;

public static class FriendshipRepository
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
    
    public static int GetSecondUserInFriendship(int USERID, int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery =
                "SELECT * FROM friendships WHERE ID = @ID;";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32("ACCEPTERID") != USERID)
                        {
                            return reader.GetInt32("ACCEPTERID");
                        }
                        if (reader.GetInt32("SENDERID") != USERID)
                        {
                            return reader.GetInt32("SENDERID");
                        }
                    }
                }
            }
        }

        throw new NGMServiceExeption(
            new ConflictObjectResult(
                "ошбика, которой не должно было произойти"));
    }
    
    public static string GetBothFriendshipIDs(int SENDERID, int ACCEPTERID)
    {
        string returner = "";
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery =
                "SELECT * FROM friendships WHERE SENDERID = @SENDERID AND ACCEPTERID = @ACCEPTERID OR SENDERID = @ACCEPTERID AND ACCEPTERID = @SENDERID;";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SENDERID", SENDERID);
                checkCommand.Parameters.AddWithValue("@ACCEPTERID", ACCEPTERID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returner += reader.GetString("ID") + " ";
                    }
                }
            }
        }
        return returner;
    }
    
    public static bool IsFriendshipMember(int ID, int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM friendships WHERE ID=@ID AND SENDERID=@USERID OR ID=@ID AND ACCEPTERID=@USERID;";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                checkCommand.Parameters.AddWithValue("@USERID", USERID);
                
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
    
    public static void DeleteUser(int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM friendships WHERE SENDERID=@USERID OR ACCEPTERID=@USERID;";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@USERID", USERID);
                
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

    public static Friendship GetFriendship(int ID)
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
                        return new Friendship
                        {
                            ID = reader.GetInt32("ID"),
                            SENDERID = reader.GetInt32("SENDERID"),
                            ACCEPTERID = reader.GetInt32("ACCEPTERID"),
                            STATE = reader.GetInt32("STATE")
                        };
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Сотружества не существует"));
                    }
                }
            }
        }
    }
}