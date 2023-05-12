namespace BotLibrary;

public interface IUpdate
{
    IChatInfo Chat { get; }

    string Text { get; }

    IUserInfo From { get; }
}