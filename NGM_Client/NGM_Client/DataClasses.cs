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
        public int id { get; set; }
        public int serverid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string roleid { get; set; }
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
    
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string displayname { get; set; }
        public string description { get; set; }
    }

    public class Server
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string joinpassword { get; set; }
        public bool visible { get; set; }
        public int ownerid { get; set; }
        public DateTime creation_date { get; set; }
    }


    public class Role
    {
        public int id { get; set; }
        public int serverid { get; set; }
        public int level { get; set; }
        public string permissions { get; set; }
        public string name { get; set; }
    }
    public class Member
    {
        public int id { get; set; }
        public int serverid { get; set; }
        public int userid { get; set; }
        public int roleid { get; set; }
        public string displayname { get; set; }
        public bool textmute { get; set; }
        public bool voicemute { get; set; }
    }


    public class Friendship
    {
        public int id { get; set; }
        public int senderid { get; set; }
        public int accepterid { get; set; }
        public int state { get; set; }
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
    public enum RoleParams
    {
        name,
        level,
        permission
    }
    public enum ServerParams
    {
        name,
        description,
        ownerid,
        joinpassword,
        visible
    }

    public enum AuthParams
    {
        username,
        password,
        email,
        displayname,
        description,
    }

    public enum MuteParams
    {
        textmute,
        voicemute
    }

    public enum ClientSections
    {
        auth,
        chats,
        friendship,
        members,
        messages,
        roles,
        servers
    }

}
