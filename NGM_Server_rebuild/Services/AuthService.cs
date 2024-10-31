using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class AuthService
{
    //Вход
    public static User Login(HttpRequest req)
    {
        //Важные переменные для функции
        string username = "";
        string password = "";
        
        //Доставание Логина\пароля из хедера
        if (req.Headers.TryGetValue("Authorization", out var auth))
        {
            var headers = AuthenticationHeaderValue.Parse(req.Headers["Authorization"]!);
            username = headers.Scheme;
            password = headers.Parameter!;
        }
        else
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Нет AUTH_HEADER"));
        }
        
        //Проверка на наличие пользователя
        using (MySqlConnection c = new MySqlConnection(gb.AuthServiceConnectionString))
        {
            c.Open();
            
            //Проверка на существование пользователя
            bool isUserExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE `userName` = @u AND `passwordHash` = @p", c))
            {
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);

                isUserExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isUserExists)
            {
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
            
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `userName` = @u AND `passwordHash` = @p", c))
            {
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new User
                        {
                            id = reader[0].ToString()??"null",
                            DisplayName = reader[1].ToString()??"null",
                            Email = reader[4].ToString()??"null",
                            Username = reader[2].ToString()??"null",
                            PasswordHash = reader[3].ToString()??"null"
                        };
                    }
                }
            }
            
            c.Close();
        }

        return new User();
    }
    //Регистрация
    public static User Register(HttpRequest req)
    {
        //Важные переменные в функции
        string username, password, email, displayName;
        
        //Доставание Логина\пароля из хедера
        if (req.Headers.TryGetValue("Authorization", out var auth))
        {
            var headers = AuthenticationHeaderValue.Parse(req.Headers["Authorization"]!);
            username = headers.Scheme;
            password = headers.Parameter!;
        }
        else
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Нет AUTH_HEADER"));
        }
        
        //Проверка хедера на наличие имейла и дисплейНейма, и их проверки на правильное написание
        if (req.Headers.TryGetValue("displayName", out var _displayName) &&
            req.Headers.TryGetValue("email", out var _email))
        {
            if (_displayName.Contains(" ") || _email.Contains(" "))
            {
                throw new NGMServiceExeption(new BadRequestObjectResult("displayName и email не может содержать пробелы"));
            }

            if (!IsValidEmail(_email))
            {
                throw new NGMServiceExeption(new BadRequestObjectResult("email не подходит"));
            }
            
            if (_displayName == "")
            {
                _displayName = username;
            }
            
            displayName = _displayName;
            email = _email;
        }
        else
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("Нет displayName или email в HEADER"));
        }
        
        //Проверка на наличие пользователя
        using (MySqlConnection c = new MySqlConnection(gb.AuthServiceConnectionString))
        {
            c.Open();
            
            //Проверка на существование пользователя
            bool isUserExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE `userName` = @u AND `passwordHash` = @p", c))
            {
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);

                isUserExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (isUserExists)
            {
                throw new NGMServiceExeption(new ConflictObjectResult("Пользователь уже существует"));
            }

            //Создание пользователя
            using (MySqlCommand command =
                   new MySqlCommand(
                       "INSERT INTO `users`(`displayName`, `userName`, `passwordHash`, `email`) VALUES (@d,@u,@p,@e)",
                       c))
            {
                command.Parameters.AddWithValue("@d", displayName);
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);
                command.Parameters.AddWithValue("@e", email);
                
                command.ExecuteNonQuery();
            }
            
            //Получение пользователя
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `userName` = @u AND `passwordHash` = @p", c))
            {
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@p", password);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new User
                        {
                            id = reader[0].ToString()??"null",
                            DisplayName = reader[1].ToString()??"null",
                            Email = reader[4].ToString()??"null",
                            Username = reader[2].ToString()??"null",
                            PasswordHash = reader[3].ToString()??"null"
                        };
                    }
                }
            }
            
            c.Close();
        }

        return new User();
    }
    
    //Функции получения пользователя
    public static User GetUser(string id)
    {
        using (MySqlConnection c = new MySqlConnection(gb.AuthServiceConnectionString))
        using (MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `id` = @id", c))
        {
            c.Open();
            
            command.Parameters.AddWithValue("@id", id);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    c.Close();
                    return new User
                    {
                        id = reader[0].ToString()??"null",
                        DisplayName = reader[1].ToString()??"null",
                        Email = reader[4].ToString()??"null",
                        Username = reader[2].ToString()??"null",
                        PasswordHash = reader[3].ToString()??"null"
                    };
                }
            }
            
            c.Close();
            return new User();
        }
    }
    
    //Локальные функции
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    
        if (Regex.IsMatch(email, pattern))
        {
            return true;
        }
        return false;
    }
}