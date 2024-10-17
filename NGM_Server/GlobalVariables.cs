using MySql.Data.MySqlClient;

namespace NGM_Server;

//todo: переработать, соединени создавать на постояннке
public static class Conf
{
    public static string DBConnection { get; set; }
}

public static class Login
{
    public static bool CheckUserLogin(string username, string hash)
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
}