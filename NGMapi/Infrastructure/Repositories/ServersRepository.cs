using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;

namespace NGMapi.Infrastructure.Repositories;

public class ServersRepository : IServersRepository
{
    public bool TryJoinWithPassword(int ID, string JOINPASSWORD)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT VISIBLE FROM servers WHERE ID=@ID AND JOINPASSWORD=@JOINPASSWORD";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", JOINPASSWORD);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    public bool IsVisible(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT VISIBLE FROM servers WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetBoolean(0);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    public bool IsServerExists(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    
    public bool IsServerExists(string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE NAME=@NAME";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    
    public Server GetServer(string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE NAME=@NAME";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Server
                        {
                            ID = reader.GetInt32("ID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            JOINPASSWORD = "Heh no passwords >:)",
                            VISIBLE = reader.GetBoolean("VISIBLE"),
                            OWNERID = reader.GetInt32("OWNERID"),
                            CREATION_DATE = reader.GetDateTime("CREATION_DATE"),
                        };
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не существует"));
                    }
                }
            }
        }
    }
    
    public Server GetServer(int SERVERID)
    {
        Console.WriteLine(SERVERID);
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", SERVERID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Server
                        {
                            ID = reader.GetInt32("ID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            JOINPASSWORD = "Heh no passwords >:)",
                            VISIBLE = reader.GetBoolean("VISIBLE"),
                            OWNERID = reader.GetInt32("OWNERID"),
                            CREATION_DATE = reader.GetDateTime("CREATION_DATE"),
                        };
                    }
                    else
                    {
                        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не существует"));
                    }
                }
            }
        }
    }
    
    public bool IsOwner(int ID, int OWNERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE ID=@ID AND OWNERID=@OWNERID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);
                insertCommand.Parameters.AddWithValue("@OWNERID", OWNERID);

                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
    
    public void CreateServer(Server server)
    {
        if (string.IsNullOrEmpty(server.DESCRIPTION))
        {
            server.DESCRIPTION = "Нет описания, ну думаю что это крутой сервер!";
        }
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "INSERT INTO servers (NAME, DESCRIPTION, JOINPASSWORD, VISIBLE, OWNERID) VALUES (@NAME, @DESCRIPTION, @JOINPASSWORD, @VISIBLE, @OWNERID)";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", server.NAME);
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", server.DESCRIPTION);
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", server.JOINPASSWORD);
                insertCommand.Parameters.AddWithValue("@VISIBLE", server.VISIBLE);
                insertCommand.Parameters.AddWithValue("@OWNERID", server.OWNERID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }

    public void ChangeName(int ID, string NAME)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET NAME=@NAME WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@NAME", NAME);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void ChangeDescription(int ID, string DESCRIPTION)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET DESCRIPTION=@DESCRIPTION WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@DESCRIPTION", DESCRIPTION);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void ChangeOwner(int ID, int OWNERID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET OWNERID=@OWNERID WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@OWNERID", OWNERID);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET JOINPASSWORD=@JOINPASSWORD WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@JOINPASSWORD", JOINPASSWORD);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void ChangeVisible(int ID, bool VISIBLE)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "UPDATE servers SET VISIBLE=@VISIBLE WHERE ID=@ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@VISIBLE", VISIBLE);
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }
    
    public void DeleteServer(int ID)
    {
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "DELETE FROM servers WHERE ID = @ID";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = insertCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return;
                }
                else
                {
                    throw new NGMServiceExeption(new ConflictObjectResult("Неизвестная ошибка"));
                }
            }
        }
    }

    public Server[] GetServers()
    {
        List<Server> servers = new List<Server>();
        
        using (MySqlConnection connection = new MySqlConnection(gb.db))
        {
            connection.Open();
            string insertQuery = "SELECT * FROM servers WHERE VISIBLE=1";

            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
            {
                using (MySqlDataReader reader = insertCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        servers.Add(new Server
                        {
                            ID = reader.GetInt32("ID"),
                            NAME = reader.GetString("NAME"),
                            DESCRIPTION = reader.GetString("DESCRIPTION"),
                            JOINPASSWORD = "0",
                            VISIBLE = reader.GetBoolean("VISIBLE"),
                            OWNERID = reader.GetInt32("OWNERID"),
                            CREATION_DATE = reader.GetDateTime("CREATION_DATE"),
                        });
                    }
                }
            }
        }
        
        return servers.ToArray();
    }
}