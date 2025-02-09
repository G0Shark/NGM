using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class RolesService
{
    public static void CreateRole(int SERVERID, int LEVEL, string[] PERMISSIONS, string NAME)
    {
        /*
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
        */
    }
}