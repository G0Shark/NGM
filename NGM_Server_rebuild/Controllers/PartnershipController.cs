using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainPartnershipController)]
public class PartnershipController : Controller
{
    //Сделать\принять запрос
    /*[HttpPost(Path.SendFriendReq)]
    public ActionResult SendFriendRequest(string OTHER_USER_ID)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string passwrod))
            {
                
            }
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }*/

    //Сделать новый контакт
    [HttpPost(Path.MakeNewPartnership)]
    public ActionResult MakeNewPartnership(string OTHER_USER_ID)
    {
        try
        {
            if (Secure.IsValidHeader(Request, out string username, out string password))
            {
                if (WhatPartnership(Secure.GetUser(username, password).id, OTHER_USER_ID, out var type))
                {
                    return Unauthorized("Контакт уже создан");
                }
                
                using (MySqlConnection c = new MySqlConnection())
                {
                    c.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `relationships`(`userId1`, `userId2`, `status`) VALUES (@uid1,@uid2,'NON')", c))
                    {
                        cmd.Parameters.AddWithValue("@uid1", Secure.GetUser(username, password).id);
                        cmd.Parameters.AddWithValue("@uid2", OTHER_USER_ID);
                        
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `relationships`(`userId1`, `userId2`, `status`) VALUES (@uid2,@uid1,'NON')", c))
                    {
                        cmd.Parameters.AddWithValue("@uid1", Secure.GetUser(username, password).id);
                        cmd.Parameters.AddWithValue("@uid2", OTHER_USER_ID);
                        
                        cmd.ExecuteNonQuery();
                    }
                    c.Close();
                }

                return Ok();
            }
            return BadRequest("No AUTH_HEADER");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //ЛОКАЛЬНЫЕ ФУНКЦИИ --------------
    private bool WhatPartnership(string own_id, string other_id, out PartnershipType partnership_type)
    {
        bool c1 = false;
        bool c2 = false;
        
        using (MySqlConnection c = new MySqlConnection(gb.dbconn))
        using (MySqlCommand cmd = new MySqlCommand("SELECT `status1` FROM `relationships` WHERE `userId1` = @uid1 AND `userId2` = @uid2", c))
        {
            cmd.Parameters.AddWithValue("@uid1", own_id);
            cmd.Parameters.AddWithValue("@uid2", other_id);
            
            c.Open();
            
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    PartnershipType pt = PartnershipType.PARTNERSHIP_NULL;
                    switch (dr[0].ToString())
                    {
                        case "PENDING_IN": pt = PartnershipType.PARTNERSHIP_PENDING_IN; break;
                        case "PENDIND_OUT": pt = PartnershipType.PARTNERSHIP_PENDING_OUT; break;
                        case "FRIEND": pt = PartnershipType.PARTNERSHIP_FRIEND; break;
                        case "BLACK_LIST": pt = PartnershipType.PARTNERSHIP_BLACKLIST; break;
                        case "NON": pt = PartnershipType.PARTNERSHIP_NONE; break;
                    }

                    if (pt != PartnershipType.PARTNERSHIP_NULL)
                    {
                        c.Close();
                        partnership_type = pt;
                        return true;
                    }
                }
            }
            c.Close();
            partnership_type = PartnershipType.PARTNERSHIP_NULL;
            return false;
        }
    }
}