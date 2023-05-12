namespace BotLibrary;

public interface IChatInfo
{
    long Id { get; }

    bool IsGroupChat { get; }
}