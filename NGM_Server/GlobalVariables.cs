using MySql.Data.MySqlClient;

namespace NGM_Server;

//todo: переработать, соединени создавать на постояннке
public static class Conf
{
    public static string DBConnection { get; set; }
}