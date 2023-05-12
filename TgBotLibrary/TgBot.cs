using BotLibrary;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotLibrary;

public class TgBot : IBot
{
    private readonly ITelegramBotClient _bot;

    public TgBot(string token)
    {
        _bot = new TelegramBotClient(token);
    }

    public void Send(long chatId, string text, Keyboard? keyboard = null, Image[]? images = null)
    {
#warning 
        _bot.SendTextMessageAsync(chatId, text);
    }

    public void DeleteKeyboard(long chatId, string text)
    {
        throw new NotImplementedException();
    }

    public string CreateMention(long userId, string name)
    {
        throw new NotImplementedException();
    }

    public event Action<IBot, IMessage>? OnMessageReceived;
    public event Action<IBot, IUpdate>? OnButtonClicked;
    public void Start()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };
        _bot.StartReceiving(
            HandleUpdate,
            HandleError,
            receiverOptions,
            cancellationToken
        );
    }

    private void HandleError(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }

    private void HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cts)
    {
        if (update.Type == UpdateType.Message && update.Message!.From != null)
        {
            OnMessageReceived?.Invoke(this, new TgMessage(update.Message));
        }
    }
}