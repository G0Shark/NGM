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
        //todo: доделать функцию (завтра)
        string sql = "SELECT id, name FROM men WHERE age = 22";
        MySqlCommand command = new MySqlCommand(sql, NGM_Server.DBConnection.connection);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine(reader[0].ToString() + " " + reader[1].ToString());
        }
        reader.Close();
    }
}