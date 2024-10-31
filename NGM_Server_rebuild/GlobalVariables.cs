namespace NGM_Server_rebuild;

// специальный класс для хранения стринги для коннетка, ибо везде открывать конфиг-билдер долго
public static class gb
{
    private static IConfigurationRoot cf = new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build();
    public static string dbconn = cf.GetSection("ConnectionStrings").GetSection("DBConnection").Value??"null";
    public static string admdbconn = cf.GetSection("ConnectionStrings").GetSection("UserDBConnection").Value??"null";
    
    //Сервисы
    public static string AuthServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("AuthServiceConnectionString").Value??"null";
    public static string PartnershipServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("PartnershipServiceConnectionString").Value??"null";
    public static string MessagingServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("MessagingServiceConnectionString").Value??"null";
    public static string ServerServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("ServerServiceConnectionString").Value??"null";
    public static string ChatServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("ChatServiceConnectionString").Value??"null";
    public static string RoleServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("RoleServiceConnectionString").Value??"null";
    public static string RolePermissionServiceConnectionString = cf.GetSection("ConnectionStrings").GetSection("RolePermissionServiceConnectionString").Value??"null";
}