namespace NGM_Server_rebuild;

public static class Path
{
    //Правила:
    //1. всё написанно на англ. ибо лень менять раскладку
    //2. все переменные написанны БОЛЬШИМИ буквами, если это в body или в header-е то указано _BODY и _HEADER
    //3. если HTTP запрос, то в коменнтарии в начале указан какой запрос, что нужно в него, и что он возвращает
    
    public static string MainAPI = "/api"; //Главный путь, стоит ВСЕГДА в начале
    
    //HUBS
    public static string MainHUB = MainAPI + "/hub";
    public static string VoiceHub = MainHUB + "/voice"; //connect with CHANNEL_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToVoiceChat with CHANNEL_ID, USERNAME, BYTE_VOICE 
    public static string ChatHub = MainHUB + "/chat"; //connect with CHANNEL_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToTextChat with CHANNEL_ID, USERNAME, MESSAGE
    
    //ChatController
    public static string MainChatController = MainAPI + "/chat";
    public static string GetChats = MainChatController + "/chats"; //GET with AUTH_HEADER || returns CHAT[], where user exists
    public static string GetMessages = MainChatController + "/messages"; //GET with AUTH_HEADER and CHAT_ID arg || returns MESSAGE[]
    
    //UserController
    public static string MainUserController = MainAPI + "/auth";
    public static string Register = MainUserController + "/register"; //POST with USER_BODY || returns USER
    public static string Login = MainUserController + "/login"; //POST with USER_BODY || returns USER
}