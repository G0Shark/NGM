using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NGM_Server.Controllers;

[ApiController]
[Route("api/db/dm")]
public class DMsController : Controller
{
    MySqlConnection c = new MySqlConnection(NGM_Server.Conf.DBConnection);

    [HttpGet("chats")]
    public IActionResult GetChats()
    {
        if (Request.Headers.TryGetValue("Username", out var Name) &&
            Request.Headers.TryGetValue("PasswordHash", out var Password))
        {
            if (Login.CheckUserLogin(Name, Password))
            {
                
            }
            else
            {
                return BadRequest("Bad user");
            }
        }
        else
        {
            return BadRequest("bad Header");
        }
    }
}