using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild;

public static class Secure
{
    //Функция проверки на наличие пользователя
    public static bool CheckUserExists(string UName)
    {
        MySqlConnection adm_connection = new MySqlConnection(gb.admdbconn);
        try
        {
            adm_connection.Open();

            using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE userName = @u", adm_connection))
            {
                command.Parameters.AddWithValue("@u", UName);

                int count = Convert.ToInt32(command.ExecuteScalar());
                adm_connection.Close();
                
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            adm_connection.Close();
            return false;
        }
    }
    
    //Функция попытки входа
    public static bool TryLogin(string UName, string PWord)
    {
        MySqlConnection adm_connection = new MySqlConnection(gb.admdbconn);
        adm_connection.Open();

        using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM users WHERE userName = @u AND passwordHash = @p", adm_connection))
        {
            command.Parameters.AddWithValue("@u", UName);
            command.Parameters.AddWithValue("@p", PWord);

            int count = Convert.ToInt32(command.ExecuteScalar());
            adm_connection.Close();
                
            return count > 0;
        }
    }
    
    //Функция получения пользователя
    public static User GetUser(string UName, string PWord)
    {
        MySqlConnection adm_connection = new MySqlConnection(gb.admdbconn);
        if (!TryLogin(UName, PWord)) { throw new Exception("Неверный Username или Password."); }
        adm_connection.Open();
        try
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `users` WHERE `userName` = @UName AND `passwordHash` = @PWord", adm_connection))
            {
                cmd.Parameters.AddWithValue("@UName", UName);
                cmd.Parameters.AddWithValue("@PWord", PWord);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new User
                        {
                            id = reader[0].ToString(),
                            DisplayName = reader[1].ToString(),
                            Email = reader[4].ToString(),
                            Username = reader[2].ToString(),
                            PasswordHash = reader[3].ToString()
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            adm_connection.Close();
            return null;
        }
        adm_connection.Close();
        return null;
    }
    
    //Функция попытки создания пользователя
    public static void CreateUser(string UName, string PWord, string DName, string Email)
    {
        MySqlConnection adm_connection = new MySqlConnection(gb.admdbconn);
        if (UName.Trim() == "" || PWord.Trim() == "" || Email.Trim() == "")
        {
            throw new Exception("Username, Password и Email не должен быть нулём или пустой строкой");
        }

        if (PWord.Length < 48)
        {
            throw new Exception("Хэш пароля не совпадает");
        }
        if (DName.Trim() == "")
        {
            DName = UName;
        }
        if (CheckUserExists(UName))
        {
            throw new Exception("Пользователь существует");
        }
        
        adm_connection.Open();

        using (MySqlCommand command = new MySqlCommand("INSERT INTO `users`(`displayName`, `userName`, `passwordHash`, `email`) VALUES (@dn,@un,@ph,@em)", adm_connection))
        {
            command.Parameters.AddWithValue("@dn", DName);
            command.Parameters.AddWithValue("@un", UName);
            command.Parameters.AddWithValue("@ph", PWord);
            command.Parameters.AddWithValue("@em", Email);

            command.ExecuteNonQuery();
            adm_connection.Close();
        }
    }
}