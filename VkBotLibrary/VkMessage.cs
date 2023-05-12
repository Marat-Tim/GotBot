using BotLibrary;
using System.Text.RegularExpressions;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;

namespace VkBotLibrary;

#warning Лучше сделать VkUpdate и VkMessage
internal class VkMessage : IMessage
{

    private readonly IVkApi _vkApi;

    public VkMessage(Message message, IVkApi vkApi) 
    {
        _vkApi = vkApi;
        Chat = new VkChatInfo(message.PeerId!.Value);
        From = new VkUserInfo(message.FromId!.Value, GetUserName(message.FromId.Value));
        if (message.Payload == null)
        {
            Text = message.Text;
        }
        else
        {
#warning вконтакте присылает вместо слэша 2 слэша
            Text = Regex.Match(message.Payload, @"{""button"":""\\(?'value'.*?)""}").Groups["value"].Value;
        }
    }
    
    public IChatInfo Chat { get; }

    public string Text { get; }

    public IUserInfo From { get; }


    public IEnumerable<MentionInfo> Mentions
    {
        get
        {
#warning В Вк другой механизм
            ICollection<MentionInfo> mentions = new List<MentionInfo>();
            var matches = Regex.Matches(this.Text, @"\[id(?'id'\d+)\|(?'name'[^\[\]]+)\]");
            try
            {
                var chatMembersProfiles = _vkApi.Messages
                    .GetConversationMembers(this.Chat.Id).Profiles;
                var chatMemberIds = chatMembersProfiles.Select(member => member.Id).ToList();
                foreach (Match match in matches)
                {
                    if (long.TryParse(match.Groups["id"].Value, out var id) && chatMemberIds.Contains(id))
                    {
                        mentions.Add(
                            new MentionInfo(
                                new VkUserInfo(id, GetUserName(id)),
                                match.Index,
                                match.Length));
                    }
                }
            }
            catch (ConversationAccessDeniedException)
            {
                throw new BotException("Бот не является администратором беседы");
            }
            return mentions;
        }
    }

    private string GetUserName(long userId)
    {
        return _vkApi.Users.Get(new[] { userId }, ProfileFields.FirstName)[0].FirstName;
    }
}