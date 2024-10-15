using MySql.Data.MySqlClient;

namespace NGM_Server;

//todo: переработать, соединени создавать на постояннке
public static class DBConnection
{
    public static MySqlConnection connection { get; set; }
}