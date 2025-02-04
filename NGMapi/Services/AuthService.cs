using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class AuthService
{
    public static bool IsUserExists(string USERNAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM users WHERE USERNAME = @Email";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@Email", USERNAME);
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
    
    public static void Register(string USERNAME, string PASSWORD, string EMAIL, string DISPLAYNAME, string DESCRIPTION)
    {
        if (string.IsNullOrEmpty(USERNAME) || string.IsNullOrEmpty(PASSWORD) || string.IsNullOrEmpty(EMAIL))
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("USERNAME, PASSWORD или EMAIL пусты"));
        }
        if (string.IsNullOrEmpty(DISPLAYNAME))
        {
            DISPLAYNAME = USERNAME;
        }

        if (string.IsNullOrEmpty(DESCRIPTION))
        {
            DESCRIPTION = "Нет описания, как минимум пока что";
        }
        
        if (IsUserExists(USERNAME))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("USERNAME занят"));
        }

        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO users (USERNAME, PASSWORD, EMAIL, DISPLAYNAME, DESCRIPTION) VALUES (@USERNAME, @PASSWORD, @EMAIL, @DISPLAYNAME, @DESCRIPTION)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                insertCommand.Parameters.AddWithValue("@PASSWORD", PASSWORD);
                insertCommand.Parameters.AddWithValue("@EMAIL", EMAIL);
                insertCommand.Parameters.AddWithValue("@DISPLAYNAME", DISPLAYNAME);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Пользователь создан"));
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }

    public static void Login(string USERNAME, string PASSWORD)
    {
        if (string.IsNullOrEmpty(USERNAME) || string.IsNullOrEmpty(PASSWORD))
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("USERNAME или PASSWORD пусты"));
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM users WHERE USERNAME = @USERNAME AND PASSWORD = @PASSWORD";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@PASSWORD", PASSWORD);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        throw new NGMServiceExeption(new OkObjectResult(new User
                        {
                                ID = int.Parse(reader["ID"].ToString()!),
                                EMAIL = reader["EMAIL"].ToString()!,
                                USERNAME = reader["USERNAME"].ToString()!,
                                DISPLAYNAME = reader["DISPLAYNAME"].ToString()!,
                                DESCRIPTION = reader["DESCRIPTION"].ToString()!
                        }));
                    }
                    else
                    {
                        throw new NGMServiceExeption(new UnauthorizedObjectResult("Неправильные данные для входа"));
                    }
                }
            }
        }
    }
}

public struct User
{
    public int ID { get; set; }
    public string USERNAME { get; set; }
    public string EMAIL { get; set; }
    public string DISPLAYNAME { get; set; }
    public string DESCRIPTION { get; set; }
}