namespace BotLibrary;

public interface IMessage : IUpdate
{

    IEnumerable<MentionInfo> Mentions { get; }
}