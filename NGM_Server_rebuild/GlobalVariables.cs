namespace NGM_Server_rebuild;

// специальный класс для хранения стринги для коннетка, ибо везде открывать конфиг-билдер долго
public static class gb
{
    private static IConfigurationRoot cf = new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build();
    public static string dbconn = cf.GetSection("ConnectionStrings").GetSection("DBConnection").Value??"null";
    public static string admdbconn = cf.GetSection("ConnectionStrings").GetSection("UserDBConnection").Value??"null";
}