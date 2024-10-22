using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : Controller
{
    [HttpGet]
    public ActionResult<string> GetChats()
    {
        MySqlConnection conn = new MySqlConnection(gb.dbconn);
        conn.Open();

        string id = "";
        if (Request.Headers.TryGetValue("Username", out var uname) && Request.Headers.TryGetValue("Password", out var psd))
        {
            id = new MySqlCommand($"SELECT `id` FROM `users` WHERE `userName` = '{uname}' AND `passwordHash` = '{psd}'", conn).ExecuteScalar().ToString();
        }
        else
        {
            conn.Close();
            return BadRequest("Bad Header");
        }
        
        List<string> chatList = new List<string>();
        MySqlDataReader r = new MySqlCommand($"SELECT `chatId` FROM `chatmembers` WHERE `userId` = '{id}'", conn)
            .ExecuteReader();
        while (r.Read())
        {
            chatList.Add(r[0].ToString());
        }
        
        return Ok(chatList.ToArray());
    }
}