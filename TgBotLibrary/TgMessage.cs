using BotLibrary;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotLibrary;

public class TgMessage : IMessage
{
    public TgMessage(Message message)
    {
        Chat = new TgChatInfo(message.Chat.Id, message.Chat.Type == ChatType.Group);
        Text = message.Text ?? "";
        From = new TgUserInfo(message.From.Id, message.From.FirstName);
        List<MentionInfo> mentions = new List<MentionInfo>();
        if (message.Entities != null)
        {
            foreach (var messageEntity in message.Entities)
            {
                if (messageEntity.Type == MessageEntityType.Mention)
                {
                    TgUserInfo user = new TgUserInfo(messageEntity.User!.Id, messageEntity.User.FirstName);
                    mentions.Add(new MentionInfo(user, messageEntity.Offset, messageEntity.Length));
                }
            }
        }
        Mentions = mentions;
    }

    public IChatInfo Chat { get; }
    public string Text { get; }
    public IUserInfo From { get; }
    public IEnumerable<MentionInfo> Mentions { get; }
}