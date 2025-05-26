using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Domain.Entities;

namespace NGM_Server_rebuild.Services;

public static class RolesRepository
{
    public static int GetLevelOfRole(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT LEVEL FROM roles WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    public static int UserMaxLevel(int USERID, int SERVERID)
    {
        var roles = GetRolesOfUser(USERID, SERVERID);
        int level = 100000;
        
        foreach (var role in roles)
        {
            if (role.LEVEL < level)
            {
                level = role.LEVEL;
            }
        }
        
        return level;
    }
    public static bool IsUserHavePermission(int USERID, int SERVERID, string PERMISSION)
    {
        var roles = GetRolesOfUser(USERID, SERVERID);
        bool hasPermission = false;
        
        foreach (var role in roles)
        {
            if (role.PERMISSIONS.Contains(PERMISSION))
            {
                hasPermission = true;
            }
        }
        
        return hasPermission;
    }
    public static Role[] GetRolesOfUser(int USERID, int SERVERID)
    {
        List<Role> roles = new List<Role>();
        string roleIdsString = "";
            using (var connection = new MySqlConnection(gb.db))
            {
                connection.Open();

                var getRoleIdsCmd = new MySqlCommand(
                    "SELECT ROLEID FROM members WHERE USERID = @USERID AND SERVERID = @SERVERID",
                    connection);
                getRoleIdsCmd.Parameters.AddWithValue("@USERID", USERID);
                getRoleIdsCmd.Parameters.AddWithValue("@SERVERID", SERVERID);

                var result = getRoleIdsCmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не участник сервера"));

                roleIdsString = result.ToString();
            }

            if (string.IsNullOrWhiteSpace(roleIdsString))
                return roles.ToArray();
            
            var roleIds = new List<int>();
            foreach (var part in roleIdsString.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part, out var id))
                    roleIds.Add(id);
            }

            if (roleIds.Count == 0)
                return roles.ToArray();
            
            using (var connection = new MySqlConnection(gb.db))
            {
                connection.Open();
                var parameters = new List<string>();
                var cmd = new MySqlCommand();
                cmd.Connection = connection;

                for (int i = 0; i < roleIds.Count; i++)
                {
                    var paramName = "@id" + i;
                    parameters.Add(paramName);
                    cmd.Parameters.AddWithValue(paramName, roleIds[i]);
                }

                var sql = $"SELECT * FROM roles WHERE ID IN ({string.Join(", ", parameters)})";
                cmd.CommandText = sql;

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new Role
                    {
                        ID = reader.GetInt32("ID"),
                        SERVERID = reader.GetInt32("SERVERID"),
                        LEVEL = reader.GetInt32("LEVEL"),
                        PERMISSIONS = reader.GetString("PERMISSIONS"),
                        NAME = reader.GetString("NAME"),
                    });
                }
            }

            return roles.ToArray();
    }
    
    public static Role GetRole(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM roles WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Role
                        {
                            ID = reader.GetInt32("ID"),
                            SERVERID = reader.GetInt32("SERVERID"),
                            LEVEL = reader.GetInt32("LEVEL"),
                            PERMISSIONS = reader.GetString("PERMISSIONS"),
                            NAME = reader.GetString("NAME"),
                        };
                    }

                    throw new NGMServiceExeption(new ConflictObjectResult("Роли не найдено."));
                }
            }
        }
    }
    
    public static bool IsLevelExists(int SERVERID, int LEVEL)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM roles WHERE SERVERID = @SERVERID AND LEVEL = @LEVEL";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                checkCommand.Parameters.AddWithValue("@LEVEL", LEVEL);
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
    
    public static int GetServerIDByRoleID(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT SERVERID FROM roles WHERE ID=@ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);
                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32("SERVERID");
                    }
                    else
                    {
                        throw new NGMServiceExeption(new BadRequestObjectResult("Данного ROLEID не найдено"));
                    }
                }
            }
        }
    }
    
    public static void CreateRole(int SERVERID, int LEVEL, string PERMISSIONS, string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO roles (SERVERID, LEVEL, PERMISSIONS, NAME) VALUES (@SERVERID, @LEVEL, @PERMISSIONS, @NAME)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SERVERID", SERVERID);
                insertCommand.Parameters.AddWithValue("@LEVEL", LEVEL);
                insertCommand.Parameters.AddWithValue("@PERMISSIONS", PERMISSIONS);
                insertCommand.Parameters.AddWithValue("@NAME", NAME);   

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
    
    public static void DeleteRole(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM roles WHERE ID=@ID";

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

    public static void DeleteAllRolesOfServer(int SERVERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM roles WHERE SERVERID=@SERVERID";

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
    
    public static void ChangeName(int ID, string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE roles SET NAME=@NAME WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

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
    
    public static void ChangeLevel(int ID, int LEVEL)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE roles SET LEVEL=@LEVEL WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@LEVEL", LEVEL);

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
    
    public static void ChangePermissions(int ID, string PERMISSIONS)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE roles SET PERMISSIONS=@PERMISSIONS WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@PERMISSIONS", PERMISSIONS);

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