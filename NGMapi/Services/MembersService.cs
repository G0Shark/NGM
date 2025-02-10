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
                insertCommand.Parameters.AddWithValue("@ROLEID", 1000000);
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
}