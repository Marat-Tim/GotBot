using BotLibrary;
using GotBot.Controllers;

namespace GotBot;

public class ExceptionsBotDecorator : IBot
{
    private readonly IBot _bot;

    public ExceptionsBotDecorator(IBot bot)
    {
        _bot = bot;
        _bot.OnMessageReceived += (bot1, message) =>
        {
            try
            {
                this.OnMessageReceived?.Invoke(bot1, message);
            }
            catch (ControllerException e)
            {
                bot1.ReplyTo(message, e.Message);
            }
            catch (BotException e)
            {
                bot1.ReplyTo(message, e.Message);
            }
        };
        _bot.OnButtonClicked += (bot1, update) =>
        {
            try
            {
                this.OnButtonClicked?.Invoke(bot1, update);
            }
            catch (ControllerException e)
            {
                bot1.ReplyTo(update, e.Message);
            }
            catch (BotException e)
            {
                bot1.ReplyTo(update, e.Message);
            }
        };
    }

    public event Action<IBot, IMessage>? OnMessageReceived;

    public event Action<IBot, IUpdate>? OnButtonClicked;

    public void Start()
    {
        _bot.Start();
    }


    public void Send(long chatId, string text, Keyboard? keyboard = null, Image[]? images = null)
    {
        _bot.Send(chatId, text, keyboard, images);
    }

    public void DeleteKeyboard(long chatId, string text)
    {
        _bot.DeleteKeyboard(chatId, text);
    }

    public string CreateMention(long userId, string name)
    {
        return _bot.CreateMention(userId, name);
    }
}