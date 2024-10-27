namespace NGM_Server_rebuild;

public static class Path
{
    //Правила:
    //1. всё написанно на англ. ибо лень менять раскладку
    //2. все переменные написанны БОЛЬШИМИ буквами, если это в body или в header-е то указано _BODY и _HEADER
    //3. если HTTP запрос, то в коменнтарии в начале указан какой запрос, что нужно в него, и что он возвращает
    
    public const string MainAPI = "/api"; //Главный путь, стоит ВСЕГДА в начале
    
    //HUBS
    public const string MainHUB = MainAPI + "/hub";
    public const string VoiceHub = MainHUB + "/voice"; //connect with CHANNEL_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToVoiceChat with CHANNEL_ID, USERNAME, BYTE_VOICE 
    public const string ChatHub = MainHUB + "/chat"; //connect with CHANNEL_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToTextChat with CHANNEL_ID, USERNAME, MESSAGE
    
    //ChatController
    public const string MainChatController = MainAPI + "/chat";
    public const string GetChats = "getchats"; //GET with AUTH_HEADER || returns CHAT[], where user exists
    public const string CreateChat = "createchat"; //POST with AUTH_HEADER and CHANNEL_NAME, CHANNEL_DESCRIPTION, CHANNEL_TYPE('DM', 'Voice', 'Message') || returns MESSAGE[]
    public const string AddToChat = "addusertochat"; //PATCH with AUTH_HEADER and CHAT_ID, OTHER_USER_ID || returns errors\ok
    
    //UserController
    public const string MainUserController = MainAPI + "/auth";
    public const string Register = "register"; //POST with USER_BODY || returns USER
    public const string Login = "login"; //POST with AUTH_HEADER || returns USER
    
    //MessageController
    public const string MainMessageController = MainAPI + "/messages";
    public const string GetMessages = "messagesfromchat"; //GET with AUTH_HEADER and CHAT_ID arg || returns MESSAGES[]
    public const string DeleteMessage = "delmsg"; //DELETE with AUTH_HEADER and MSG_ID arg || returns errors\ok
    public const string ModifyMessage = "modifmsg"; //PATCH with AUTH_HEADER and MSG_ID, NEW_CONTENT args || returns errors\ok
}