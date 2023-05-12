using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageControllers;

public class StartGame : CommandWithArgs
{
    private const int PlayersCount = 6;

    private readonly IGamesManager _gamesManager;

    public StartGame(IGamesManager gamesManager)
        : base(1 + PlayersCount + 1, "/start_game", "/начать_игру")
    {
        _gamesManager = gamesManager;
    }

    protected override void Control(IBot bot, IMessage message, IReadOnlyList<string> args)
    {
        if (!message.Chat.IsGroupChat)
        {
            throw new ControllerException("Команда не может быть использована в личном чате");
        }
        Game? game = _gamesManager.GetGameByChatId(message.Chat.Id);
        if (game != null)
        {
            throw new ControllerException("В этом чате уже есть игра");
        }
        if (!int.TryParse(args[^1], out int trustLevel))
        {
            throw new ControllerException("Последний аргумент должен быть числом");
        }
        MentionInfo[] mentions = message.Mentions.ToArray();
        if (mentions.Length != PlayersCount)
        {
            throw new ControllerException("Неправильное количество игроков");
        }
        if (mentions[0].Start != args[0].Length + 1)
        {
            throw new ControllerException("Неправильно указан 1-ый игрок");
        }
        for (int i = 1; i < mentions.Length; i++)
        {
            if (mentions[i].Start != mentions[i - 1].Start + mentions[i - 1].Length + 1)
            {
                throw new ControllerException($"Неправильно указан {i}-ый игрок");
            }
        }
        if (trustLevel <= 0 || trustLevel > mentions.Length)
        {
            throw new ControllerException("Неправильный уровень доверия");
        }
        if (mentions.Select(mention => mention.User.Id).ToHashSet().Count != PlayersCount)
        {
            throw new ControllerException("В списке игроков есть повторения");
        }
        int gameNumber = _gamesManager.StartGame(message.Chat.Id, mentions.Select(mention => mention.User), trustLevel);
        bot.ReplyTo(message, $"Начинаем партию номер {gameNumber}", Keyboards.GameKeyboard);
    }
}