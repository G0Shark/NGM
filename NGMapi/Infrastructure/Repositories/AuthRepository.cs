using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;

namespace NGMapi.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private bool IsUserExists(string USERNAME)
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
                        return true;
                    return false;
                }
            }
        }
    }
    
    public void Register(User user)
    {
        if (IsUserExists(user.USERNAME))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("USERNAME занят"));
        }

        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO users (USERNAME, PASSWORD, EMAIL, DISPLAYNAME, DESCRIPTION) VALUES (@USERNAME, @PASSWORD, @EMAIL, @DISPLAYNAME, @DESCRIPTION)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@USERNAME", user.USERNAME);
                insertCommand.Parameters.AddWithValue("@PASSWORD", user.PASSWORD);
                insertCommand.Parameters.AddWithValue("@EMAIL", user.EMAIL);
                insertCommand.Parameters.AddWithValue("@DISPLAYNAME", user.DISPLAYNAME);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", user.DESCRIPTION);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Пользователь создан"));
                }
                throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
            }
        }
    }

    public User GetUser(string USERNAME)
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
                            DESCRIPTION = reader["DESCRIPTION"].ToString()!,
                        };
                    }
                }
            }
        }
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public User GetUserWithPassword(string USERNAME)
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
                            DESCRIPTION = reader["DESCRIPTION"].ToString()!,
                            PASSWORD =  reader["PASSWORD"].ToString()!,
                        };
                    }
                }
            }
        }
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public User GetUser(int ID)
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
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public void DeleteUser(string USERNAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "DELETE FROM users WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Пользователь создан"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }
    public void UpdateUsername(string USERNAME, string NEWUSERNAME)
    {
        if (IsUserExists(NEWUSERNAME))
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("username занят"));
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "UPDATE users SET USERNAME = @NEWUSERNAME WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@NEWUSERNAME", NEWUSERNAME);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Ваш username изменён"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }
    
    public void UpdatePassword(string USERNAME, string PASSWORD)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "UPDATE users SET PASSWORD = @PASSWORD WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@PASSWORD", PASSWORD);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Ваш password изменён"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }

    public void UpdateEmail(string USERNAME, string EMAIL)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "UPDATE users SET EMAIL = @EMAIL WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@EMAIL", EMAIL);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Ваш email изменён"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }
    
    public void UpdateDisplayName(string USERNAME, string DISPLAYNAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "UPDATE users SET DISPLAYNAME = @DISPLAYNAME WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@DISPLAYNAME", DISPLAYNAME);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Ваш displayname изменён"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }
    
    public void UpdateDescription(string USERNAME, string DESCRIPTION)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string checkQuery = "UPDATE users SET DESCRIPTION = @DESCRIPTION WHERE USERNAME = @USERNAME";

            using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@USERNAME", USERNAME);
                checkCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);

                int rowsAffected = checkCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    throw new NGMServiceExeption(new OkObjectResult("Ваш description изменён"));
                }
                throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
            }
        }
    }
}