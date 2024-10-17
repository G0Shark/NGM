using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server.Controllers;

[ApiController]
[Route("api/db/users")]
public class UsersController : Controller
{
    MySqlConnection c = new MySqlConnection(NGM_Server.Conf.DBConnection);

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserForm u)
    {
        if (u.Username == "" || u.PasswordHash == "")
        {
            return BadRequest("No Username or Password");
        }

        if (!IsSafeString(u.Username) || !IsSafeString(u.PasswordHash))
        {
            return BadRequest("sql injection");
        }
        
        if (CheckUserExists(u.Username))
        {
            if (CheckUserLogin(u.Username, u.PasswordHash))
            {
                return Ok("Login successfull");
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
        }
        else
        {
            return BadRequest("User doesn't exists");
        }
    }
    
    [HttpPost("register")]
    public IActionResult CreateUser([FromBody] UserForm u)
    {
        if (u.Username == "" || u.PasswordHash == "")
        {
            return BadRequest("Cannot make no name user or no password user");
        }
        if (u.DisplayName == "")
        {
            u.DisplayName = u.Username;
        }
        if (u.Email == "")
        {
            return BadRequest("Cannot create user without email");
        }
        
        if (!IsSafeString(u.Username) || !IsSafeString(u.PasswordHash) || !IsSafeString(u.Email) || !IsSafeString(u.DisplayName))
        {
            return BadRequest("sql injection");
        }
        
        //Проверка на наличие пользователя
        if (!CheckUserExists(u.Username))
        {
            c.Open();
            //некая защита
             try
            {
                new MySqlCommand($"INSERT INTO `users`(`Username`, `DisplayName`, `Email`, `PasswordHash`) VALUES ('{u.Username}','{u.DisplayName}','{u.Email}','{u.PasswordHash}')", c).ExecuteScalar();
                c.Close();
                return Ok("User created");
            }
            catch (Exception e)
            {
                c.Close();
                return BadRequest("Throwed exeption: "+e.Message);
            }
        }
        else
        {
            c.Close();
            return BadRequest("User exists");
        }
    }
    
    private bool CheckUserExists(string username)
    {
        using (var connection = new MySqlConnection(Conf.DBConnection))
        {
            connection.Open();

            string query = $"SELECT COUNT(*) FROM `users` WHERE `username` = '{username}'";

            using (var command = new MySqlCommand(query, connection))
            {
                var reader = command.ExecuteReader();
                reader.Read();

                int count = int.Parse(reader[0].ToString()??"0");

                connection.Close();
                
                return count > 0;
            }
        }
    }

    private bool CheckUserLogin(string username, string hash)
    {
        using (var connection = new MySqlConnection(Conf.DBConnection))
        {
            connection.Open();

            string query = $"SELECT COUNT(*) FROM `users` WHERE `username` = '{username}' AND `PasswordHash` = '{hash}'";

            using (var command = new MySqlCommand(query, connection))
            {
                var reader = command.ExecuteReader();
                reader.Read();

                int count = int.Parse(reader[0].ToString()??"0");

                connection.Close();
                
                return count > 0;
            }
        }
    }
    private static bool IsSafeString(string input)
    {
        // Check for common SQL injection patterns
        string[] sqlPatterns = { "@@", "kill", "--", ";", "/*", "*/", "=" };
    
        foreach (string pattern in sqlPatterns)
        {
            if (input.Contains(pattern))
            {
                return false;
            }
        }

        // Additional checks
        if (input.Length > 500 || input.Contains("\r\n"))
        {
            return false;
        }

        return true;
    }
}

//Форма для создания юзера
public class UserForm
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
}