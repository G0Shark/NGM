using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;

namespace NGMapi.Infrastructure.Repositories;


public class MembersRepository : IMembersRepository
{
    public void CreateMember(int SERVERID, int USERID, string DISPLAYNAME)
    {
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
    
    public void SetTextMute(int USERID, int SERVERID, bool VALUE)
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
    
    public void SetVoiceMute(int USERID, int SERVERID, bool VALUE)
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

    public bool IsMember(int SERVERID, int USERID)
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
    
    public bool IsTextMuted(int SERVERID, int USERID)
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
    
    public bool IsVoiceMuted(int SERVERID, int USERID)
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
    public void DeleteAllMembersOfServer(int SERVERID)
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
    
    public void Delete(int SERVERID, int USERID)
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
    
    public void DeleteUser(int USERID)
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

    public Member GetMember(int ID)
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

    public Member[] GetMembersOfServer(int SERVERID)
    {
        List<Member> members = new List<Member>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM members WHERE SERVERID = @SERVERID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SERVERID", SERVERID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new Member
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            USERID = reader.GetInt32("USERID"),
                            ROLEID = reader.GetInt32("ROLEID"),
                            DISPLAYNAME = reader.GetString("DISPLAYNAME"),
                            TEXTMUTE = reader.GetBoolean("TEXTMUTE"),
                            VOICEMUTE = reader.GetBoolean("VOICEMUTE"),
                        });
                    }
                }
            }
        }
        
        return members.ToArray();
    }
    
    public Member[] GetMembersOfUser(int USERID)
    {
        List<Member> members = new List<Member>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM members WHERE USERID = @USERID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERID", USERID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        members.Add(new Member
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            USERID = reader.GetInt32("USERID"),
                            ROLEID = reader.GetInt32("ROLEID"),
                            DISPLAYNAME = reader.GetString("DISPLAYNAME"),
                            TEXTMUTE = reader.GetBoolean("TEXTMUTE"),
                            VOICEMUTE = reader.GetBoolean("VOICEMUTE"),
                        });
                    }
                }
            }
        }
        
        return members.ToArray();
    }
}