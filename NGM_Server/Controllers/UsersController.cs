using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace NGM_Server.Controllers;

[ApiController]
[Route("api/db/users")]
public class UsersController : Controller
{
    [HttpGet("post")]
    public IActionResult GetUser(string username, string password)
    {
        MySqlConnection c = new MySqlConnection(NGM_Server.Conf.DBConnection);
        c.Open();
        //todo: доделать функцию (завтра)
        MySqlCommand command = new MySqlCommand($"SELECT * FROM `users` WHERE `name` = '{username}' AND `password` = '{password}'", c);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine(reader[2].ToString() + " " + reader[3].ToString());
        }
        reader.Close();
        c.Close();

        return BadRequest();
    }
}