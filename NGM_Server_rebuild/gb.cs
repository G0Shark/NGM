namespace NGM_Server_rebuild;

// специальный класс для хранения стринги для коннетка, ибо везде открывать конфиг-билдер долго
public static class gb
{
    public static string dbconn = new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build().GetSection("ConnectionStrings").GetSection("DBConnection").Value??"null";
}