using BotLibrary;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Color = BotLibrary.Color;

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
        _bot.SendTextMessageAsync(
            chatId,
            text,
            replyMarkup: keyboard == null
                ? null
                : new ReplyKeyboardMarkup(keyboard.Buttons
                    .Select(
                        buttons => buttons.Select(ConvertToTgButton)
                    )
                )
                {
                    ResizeKeyboard = true
                },
            parseMode: ParseMode.Markdown
        );
        if (images != null)
        {
            List<IAlbumInputMedia> album = new List<IAlbumInputMedia>();
            int i = 0;
            foreach (var image in images)
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(image.Data, 0, image.Data.Length);
                stream.Position = 0;
                album.Add(new InputMediaPhoto(InputFile.FromStream(stream, $"file{i}.jpg")));
                ++i;
            }
            _bot.SendMediaGroupAsync(chatId, album);
        }
    }

    public void DeleteKeyboard(long chatId, string text)
    {
        _bot.SendTextMessageAsync(chatId, text, replyMarkup: new ReplyKeyboardRemove());
    }

    public string CreateMention(long userId, string name)
    {
        return $"[{name}](tg://user?id={userId})";
    }

    public event Action<IBot, IMessage>? OnMessageReceived;
    public event Action<IBot, IUpdate>? OnButtonClicked;
    public void Start()
    {
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {

        };
        _bot.StartReceiving(
            HandleUpdate,
            HandleError,
            receiverOptions,
            cancellationToken
        );
        cancellationToken.WaitHandle.WaitOne();
    }

    private void HandleError(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
#warning игнорирую ошибку
    }

    private void HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken cts)
    {
#warning Не понятно когда From может быть null
        if (update.Type == UpdateType.Message && update.Message!.From != null && update.Message.Text != null)
        {
            if (update.Message.Text.EndsWith("\u2800"))
            {
                var text = update.Message.Text.Substring(0, update.Message.Text.Length - 1);
                if (update.Message.Text.StartsWith("\ud83d\udd34 ") ||
                    update.Message.Text.StartsWith("\ud83d\udfe2 ") ||
                    update.Message.Text.StartsWith("\u25ef "))
                {
                    text = text.Substring(text.IndexOf(' ') + 1);
                }

                IUpdate msg = new TgMessage(new Message()
                {
                    Text = text,
                    From = update.Message.From,
                    Chat = update.Message.Chat,
                    Entities = update.Message.Entities
                });
                OnButtonClicked?.Invoke(this, msg);
            }
            else
            {
                OnMessageReceived?.Invoke(this, new TgMessage(update.Message));
            }
        }
    }

    private KeyboardButton ConvertToTgButton(Button button)
    {
        return new KeyboardButton(ConvertColor(button.Color) + button.Text + "\u2800");
    }

    private static string ConvertColor(Color color)
    {
        switch (color)
        {
            case Color.Primary:
                return "\u25ef ";
            case Color.Positive:
                return "\ud83d\udfe2 ";
            case Color.Negative:
                return "\ud83d\udd34 ";
            case Color.Without:
                return "";
        }

        throw new Exception("Добавь новое преобразование");
    }
}