using K4os.Compression.LZ4.Internal;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;


public static class MembersService
{
    public static void Create(int SERVERID, int USERID, string? DISPLAYNAME)
    {
        if (string.IsNullOrEmpty(DISPLAYNAME))
        {
            DISPLAYNAME = AuthService.GetUser(USERID).DISPLAYNAME;
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO members(SERVERID, USERID, ROLEID, DISPLAYNAME) VALUES (@SERVERID,@USERID,@ROLEID,@DISPLAYNAME)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);
                insertCommand.Parameters.AddWithValue("@ROLEID", -1);
                insertCommand.Parameters.AddWithValue("@DISPLAYNAME", DISPLAYNAME);

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
    
    public static void SetTextMute(int USERID, int SERVERID, bool VALUE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE members SET TEXTMUTE=@VALUE WHERE USERID=@USERID AND SERVERID=@SERVERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@VALUE", VALUE);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);

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
    
    public static void SetVoiceMute(int USERID, int SERVERID, bool VALUE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE members SET VOICEMUTE=@VALUE WHERE USERID=@USERID AND SERVERID=@SERVERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@VALUE", VALUE);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);

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

    public static bool IsMember(int SERVERID, int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM members WHERE USERID=@USERID AND SERVERID=@SERVERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);

                using var reader = insertCommand.ExecuteReader();
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
    
    public static bool IsTextMuted(int SERVERID, int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM members WHERE USERID=@USERID AND SERVERID=@SERVERID AND TEXTMUTE=1";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);

                using var reader = insertCommand.ExecuteReader();
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
    
    public static bool IsVoiceMuted(int SERVERID, int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM members WHERE USERID=@USERID AND SERVERID=@SERVERID AND VOICEMUTE=1";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@USERID", USERID);

                using var reader = insertCommand.ExecuteReader();
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
    public static void Delete(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM members WHERE ID=@ID";

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
    
    public static void DeleteAllMembersOfServer(int SERVERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM members WHERE SERVERID=@SERVERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);

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
    
    public static void Delete(int SERVERID, int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM members WHERE SERVERID=@SERVERID AND USERID=@USERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
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
    
    public static void DeleteUser(int USERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM members WHERE USERID=@USERID";

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

    public static Member GetMember(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM members WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Member
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            USERID = reader.GetInt32("USERID"),
                            ROLEID = reader.GetInt32("ROLEID"),
                            DISPLAYNAME = reader.GetString("DISPLAYNAME"),
                            TEXTMUTE = reader.GetBoolean("TEXTMUTE"),
                            VOICEMUTE = reader.GetBoolean("VOICEMUTE"),
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

public class Member
{
    public int ID { get; set; }
    public int SERVERID { get; set; }
    public int USERID { get; set; }
    public int ROLEID { get; set; }
    public string DISPLAYNAME { get; set; }
    public bool TEXTMUTE { get; set; }
    public bool VOICEMUTE { get; set; }
}