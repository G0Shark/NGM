using Google.Protobuf.WellKnownTypes;
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
        public int id { get; set; }
        public int chatid { get; set; }
        public string message { get; set; }
        public int replyforid { get; set; }
        public Timestamp timestamp { get; set; }
        public bool dmchat { get; set; }
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
    public enum FriendshipActions
    {
        make,
        sendreq,
        block,
        unblock
    }
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string displayname { get; set; }
        public string description { get; set; }
    }

}
