using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild;

public static class Secure
{
    public static bool IsValidHeader(HttpRequest req, out string username, out string password)
    {
        username = "";
        password = "";
        if (req.Headers.TryGetValue("Authorization", out var auth))
        {
            var headers = AuthenticationHeaderValue.Parse(req.Headers["Authorization"]!);
            username = headers.Scheme;
            password = headers.Parameter;
            return true;
        }
        return false;
    }
    
    //Функция проверки на наличие пользователя
    public static bool CheckUserExists(string uName)
    {
        MySqlConnection admConnection = new MySqlConnection(gb.admdbconn);
        try
        {
            admConnection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE userName = @u", admConnection))
            {
                command.Parameters.AddWithValue("@u", uName);

                int count = Convert.ToInt32(command.ExecuteScalar());
                admConnection.Close();
                
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            admConnection.Close();
            return false;
        }
    }
    
    //Функция попытки входа
    public static bool TryLogin(string uName, string pWord)
    {
        MySqlConnection admConnection = new MySqlConnection(gb.admdbconn);
        admConnection.Open();
        
        using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE `userName` = @u AND `passwordHash` = @p", admConnection))
        {
            command.Parameters.AddWithValue("@u", uName);
            command.Parameters.AddWithValue("@p", pWord);
            
            int count = Convert.ToInt32(command.ExecuteScalar());
            
            admConnection.Close();
                
            return count > 0;
        }
    }
    
    //Функция получения пользователя
    public static User GetUser(string uName, string pWord)
    {
        MySqlConnection admConnection = new MySqlConnection(gb.admdbconn);
        if (!TryLogin(uName, pWord)) { throw new Exception("Неверный Username или Password."); }
        admConnection.Open();
        try
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `users` WHERE `userName` = @u AND `passwordHash` = @p", admConnection))
            {
                cmd.Parameters.AddWithValue("@u", uName);
                cmd.Parameters.AddWithValue("@p", pWord);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new User
                        {
                            id = reader[0].ToString(),
                            DisplayName = reader[1].ToString(),
                            Email = reader[4].ToString(),
                            Username = reader[2].ToString(),
                            PasswordHash = reader[3].ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            admConnection.Close();
            return null;
        }
        admConnection.Close();
        return null;
    }
    
    //Функция попытки создания пользователя
    public static void CreateUser(string uName, string pWord, string dName, string email)
    {
        MySqlConnection admConnection = new MySqlConnection(gb.admdbconn);
        if (uName.Trim() == "" || pWord.Trim() == "" || email.Trim() == "")
        {
            throw new Exception("Username, Password и Email не должен быть нулём или пустой строкой");
        }

        if (pWord.Length < 48)
        {
            throw new Exception("Хэш пароля не совпадает");
        }
        if (dName.Trim() == "")
        {
            dName = uName;
        }
        if (CheckUserExists(uName))
        {
            throw new Exception("Пользователь существует");
        }
        
        admConnection.Open();

        using (MySqlCommand command = new MySqlCommand("INSERT INTO `users`(`displayName`, `userName`, `passwordHash`, `email`) VALUES (@dn,@un,@ph,@em)", admConnection))
        {
            command.Parameters.AddWithValue("@dn", dName);
            command.Parameters.AddWithValue("@un", uName);
            command.Parameters.AddWithValue("@ph", pWord);
            command.Parameters.AddWithValue("@em", email);

            command.ExecuteNonQuery();
            admConnection.Close();
        }
    }
}