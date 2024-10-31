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
    public const string VoiceHub = MainHUB + "/voice"; //connect with CHAT_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToVoiceChat with CHAT_ID, USERNAME, BYTE_VOICE 
    public const string ChatHub = MainHUB + "/chat"; //connect with CHAT_ID, USERNAME, PASSWORD || disconnect with CHANELL_ID, USERNAME, PASSWORD || SendToTextChat with CHAT_ID, USERNAME, MESSAGE
    
    
}