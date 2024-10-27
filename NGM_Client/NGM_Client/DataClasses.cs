using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM_Client
{
    public class Chat
    {
        public string id { get; set; }
        public string channelName { get; set; }
        public string channelDescription { get; set; }
        public string channelType { get; set; }
    }
    public class Message
    {
        public string id { get; set; }
        public string chatId { get; set; }
        public string senderId { get; set; }
        public string time { get; set; }
        public string content { get; set; }
        public string answeringMessageId { get; set; }
    }
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
    public class User
    {
        public string id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
