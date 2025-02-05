using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server_rebuild.Services;

public static class AuthService
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        
        string pattern = @"^[^@s]+@[^@s]+.[^@s]+$";
        Regex regex = new Regex(pattern);
        
        return regex.IsMatch(email);
    }

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
    
    public static bool IsUserExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM users WHERE ID = @ID";

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

        if (!IsValidEmail(EMAIL))
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Неверная почта"));
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

    public static User GetUser(string USERNAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM users WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            ID = int.Parse(reader["ID"].ToString()!),
                            EMAIL = reader["EMAIL"].ToString()!,
                            USERNAME = reader["USERNAME"].ToString()!,
                            DISPLAYNAME = reader["DISPLAYNAME"].ToString()!,
                            DESCRIPTION = reader["DESCRIPTION"].ToString()!
                        };
                    }
                }
            }
        }

        throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
    }
    public static User GetUser(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "SELECT * FROM users WHERE ID = @ID";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = checkCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            ID = int.Parse(reader["ID"].ToString()!),
                            EMAIL = reader["EMAIL"].ToString()!,
                            USERNAME = reader["USERNAME"].ToString()!,
                            DISPLAYNAME = reader["DISPLAYNAME"].ToString()!,
                            DESCRIPTION = reader["DESCRIPTION"].ToString()!
                        };
                    }
                }
            }
        }
        
        throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
    }

    public static string TryLogin(HttpRequest Request)
    {
        if (!Request.Headers.TryGetValue("Authorization", out var auth))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("Нет header-а authorization"));
        }
        
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        
        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var headerValue))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("Auth Header неправильный"));
        }
        
        if (!"Basic".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("Auth Header неверная разметка"));
        }
        
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter)).Split(':', 2);
        
        if (credentials.Length != 2)
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("Auth Header неверное кол-во записей"));
        }
        
        var USERNAME = credentials[0];
        var PASSWORD = credentials[1];
        
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
                        return USERNAME;
                    }
                    else
                    {
                        throw new NGMServiceExeption(new UnauthorizedObjectResult("Неверные данные для входа"));
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