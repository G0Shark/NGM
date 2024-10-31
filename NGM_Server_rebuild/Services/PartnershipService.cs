using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Services;

public static class PartnershipService
{
    //Создать отношение с человеком
    public static void CreatePartnership(string own_id, string other_id)
    {
        //Проверка на наличие отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();
            
            bool isPartnershipExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `relationships` WHERE `userId1` = @u1 AND `userId2` = @u2", connection))
            {
                command.Parameters.AddWithValue("@u1", own_id);
                command.Parameters.AddWithValue("@u2", other_id);

                isPartnershipExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (isPartnershipExists)
            {
                throw new NGMServiceExeption(new NotFoundObjectResult("Отношения уже созданны"));
            }
            
            connection.Close();
        }
        
        //Создание отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(
                       "INSERT INTO `relationships`(`userId1`, `userId2`, `status`) VALUES (@ownid,@otherid,'NONE');\n"
                    + "INSERT INTO `relationships`(`userId1`, `userId2`, `status`) VALUES (@otherid,@ownid,'NONE')", connection))
            {
                command.Parameters.AddWithValue("@ownid", own_id);
                command.Parameters.AddWithValue("@otherid", other_id);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Получить отношения с челом
    public static Relationships GetRelationships(string own_id, string other_id)
    {
        Relationship rship1 = new Relationship(); Relationship rship2= new Relationship();

        //Проверка на наличие отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();
            
            bool isPartnershipExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `relationships` WHERE `userId1` = @u1 AND `userId2` = @u2", connection))
            {
                command.Parameters.AddWithValue("@u1", own_id);
                command.Parameters.AddWithValue("@u2", other_id);

                isPartnershipExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (!isPartnershipExists)
            {
                throw new NGMServiceExeption(new NotFoundObjectResult("Отношений не созданно"));
            }
            
            connection.Close();
        }
        
        //Получение отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();
            
            //Ваши отношения
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `relationships` WHERE `userId1` = @ownid AND `userId2` = @otherid", connection))
            {
                command.Parameters.AddWithValue("@ownid", own_id);
                command.Parameters.AddWithValue("@otherid", other_id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PartnershipType type1 = PartnershipType.PARTNERSHIP_NULL;

                        switch (reader[3].ToString())
                        {
                            case "NON": type1 = PartnershipType.PARTNERSHIP_NONE; break;
                            case "PENDING_IN": type1 = PartnershipType.PARTNERSHIP_PENDING_IN; break;
                            case "PENDING_OUT": type1 = PartnershipType.PARTNERSHIP_PENDING_OUT; break;
                            case "FRIEND": type1 = PartnershipType.PARTNERSHIP_FRIEND; break;
                            case "BLACKLIST": type1 = PartnershipType.PARTNERSHIP_BLACKLIST; break;
                        }
                        
                        rship1 = new Relationship
                        {
                            innerUserId = own_id,
                            outerUserId = other_id,
                            partnershipId = reader[0].ToString()??"null",
                            status = type1
                        };
                    }
                }
            }
            
            //Чужие отношения
            using (MySqlCommand command = new MySqlCommand("SELECT * FROM `relationships` WHERE `userId1` = @otherid AND `userId2` = @ownid", connection))
            {
                command.Parameters.AddWithValue("@ownid", own_id);
                command.Parameters.AddWithValue("@otherid", other_id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PartnershipType type1 = PartnershipType.PARTNERSHIP_NULL;

                        switch (reader[3].ToString())
                        {
                            case "NON": type1 = PartnershipType.PARTNERSHIP_NONE; break;
                            case "PENDING_IN": type1 = PartnershipType.PARTNERSHIP_PENDING_IN; break;
                            case "PENDING_OUT": type1 = PartnershipType.PARTNERSHIP_PENDING_OUT; break;
                            case "FRIEND": type1 = PartnershipType.PARTNERSHIP_FRIEND; break;
                            case "BLACKLIST": type1 = PartnershipType.PARTNERSHIP_BLACKLIST; break;
                        }
                        
                        rship2 = new Relationship
                        {
                            innerUserId = other_id,
                            outerUserId = own_id,
                            partnershipId = reader[0].ToString()??"null",
                            status = type1
                        };
                    }
                }
            }
            
            connection.Close();

            return new Relationships
            {
                own = rship1,
                other = rship2
            };
        }
    }
    
    //Отправить\Принять запрос в друзья
    public static void SendOrAcceptFriendRequest(string own_id, string other_id)
    {
        Relationships relationships = GetRelationships(own_id, other_id);

        //Проверка на статус
        if (relationships.own.status == PartnershipType.PARTNERSHIP_BLACKLIST)
        {
            throw new NGMServiceExeption(new ConflictObjectResult("Данный пользователь заблокирован"));
        }
        if (relationships.other.status == PartnershipType.PARTNERSHIP_BLACKLIST)
        {
            throw new NGMServiceExeption(new ConflictObjectResult("Вы заблокированы для данного пользователя"));
        }
        if (relationships.other.status == PartnershipType.PARTNERSHIP_PENDING_OUT)
        {
            throw new NGMServiceExeption(new ConflictObjectResult("Вы уже отправили запрос в друзья"));
        }
        if (relationships.own.status == PartnershipType.PARTNERSHIP_FRIEND && relationships.other.status == PartnershipType.PARTNERSHIP_FRIEND)
        {
            throw new NGMServiceExeption(new ConflictObjectResult("Вы и так в друзьях"));
        }

        //Изменение статуса
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();

            if (relationships.own.status == PartnershipType.PARTNERSHIP_PENDING_IN)
            {
                using (MySqlCommand command = new MySqlCommand(
                           "UPDATE `relationships` SET `status`='FRIEND' WHERE `userId1` = @uid AND `userId2` = @oid;\n"
                           + "UPDATE `relationships` SET `status`='FRIEND' WHERE `userId1` = @oid AND `userId2` = @uid", 
                           connection))
                {
                    command.Parameters.AddWithValue("@uid", own_id);
                    command.Parameters.AddWithValue("@oid", other_id);
                    
                    command.ExecuteNonQuery();
                }
            }
            if (relationships.own.status == PartnershipType.PARTNERSHIP_NONE)
            {
                using (MySqlCommand command = new MySqlCommand(
                           "UPDATE `relationships` SET `status`='PENDIND_OUT' WHERE `userId1` = @uid AND `userId2` = @oid;\n"
                           + "UPDATE `relationships` SET `status`='PENDING_IN' WHERE `userId1` = @oid AND `userId2` = @uid", 
                           connection))
                {
                    command.Parameters.AddWithValue("@uid", own_id);
                    command.Parameters.AddWithValue("@oid", other_id);
                    
                    command.ExecuteNonQuery();
                }
            }
            
            connection.Close();
        }
    }
    
    //Отправить пользователя в чёрный список
    public static void SendUserToBackList(string own_id, string other_id)
    {
        //Проверка на наличие отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();
            
            bool isPartnershipExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `relationships` WHERE `userId1` = @u1 AND `userId2` = @u2", connection))
            {
                command.Parameters.AddWithValue("@u1", own_id);
                command.Parameters.AddWithValue("@u2", other_id);

                isPartnershipExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (isPartnershipExists)
            {
                CreatePartnership(own_id, other_id);
            }
            
            connection.Close();
        }
        
        //Добавление пользователя в чёрный список
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(
                       "UPDATE `relationships` SET `status`='BLACKLIST' WHERE `userId1`=@ownid AND `userId2`=@otherid;", connection))
            {
                command.Parameters.AddWithValue("@ownid", own_id);
                command.Parameters.AddWithValue("@otherid", other_id);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
    
    //Сбросить отношения
    public static void SwitchToNone(string own_id, string other_id)
    {
        //Проверка на наличие отношений
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();
            
            bool isPartnershipExists = false;
            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `relationships` WHERE `userId1` = @u1 AND `userId2` = @u2", connection))
            {
                command.Parameters.AddWithValue("@u1", own_id);
                command.Parameters.AddWithValue("@u2", other_id);

                isPartnershipExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            if (isPartnershipExists)
            {
                CreatePartnership(own_id, other_id);
            }
            
            connection.Close();
        }
        
        //Изменение отношений на нейтральные
        using (MySqlConnection connection = new MySqlConnection(gb.PartnershipServiceConnectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand(
                       "UPDATE `relationships` SET `status`='NONE' WHERE `userId1`=@ownid AND `userId2`=@otherid;", connection))
            {
                command.Parameters.AddWithValue("@ownid", own_id);
                command.Parameters.AddWithValue("@otherid", other_id);
                
                command.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
}