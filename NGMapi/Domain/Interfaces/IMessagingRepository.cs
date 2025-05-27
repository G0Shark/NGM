using NGM_Server_rebuild.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IMessagingRepository
{
    public Message[] GetMessages(int chatId, bool isDm);
    public Message[] GetMessagesFromDM(int chatId1, int chatId2);
    public Message GetMessage(int Id);
    public Message SendMessage(Message message);
    public void EditMessage(int id, string content);
    public void DeleteMessage(int id);
    public bool IsMessageExists(int id);
    public bool IsUserAreSender(int id, int senderID);
}