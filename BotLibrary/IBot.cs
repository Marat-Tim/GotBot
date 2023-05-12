namespace BotLibrary;

public interface IBot
{
    public void Send(long chatId, string text, Keyboard? keyboard = null, Image[]? images = null);

    void DeleteKeyboard(long chatId, string text);

    void ReplyTo(IUpdate update, string text, Keyboard? keyboard = null, Image[]? images = null) =>
        Send(update.Chat.Id, text, keyboard, images);

    string CreateMention(long userId, string name);

    event Action<IBot, IMessage> OnMessageReceived;

    event Action<IBot, IUpdate> OnButtonClicked;

    void Start();
}