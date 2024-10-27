namespace NGM_Server_rebuild.Models;

public enum Permission
{
    ALL = 'a',
    DELETE = 'd',
    CHANGE = 'c',
    CHAT_NAME = 'n',
    CHAT_NEEDED_PERMS = 'p',
    CHAT_DESC = 's',
    CHAT_ADDPEOPLE = 'u',
    CHAT_DELPEOPLE = 'e',
    DELETING_MSG = 'm',
    VOICE_NAME = 'v',
    VOICE_DESC = 'w',
    VOICE_MUTE = 'j',
    VOICE_KICK = 'k'
}