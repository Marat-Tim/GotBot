using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageControllers;

public class Battle : CommandWithArgs
{
    private readonly IGamesManager _gamesManager;

    public Battle(IGamesManager gamesManager) : base(3, "/battle", "/бой")
    {
        _gamesManager = gamesManager;
    }

    protected override void Control(IBot bot, IMessage message, IReadOnlyList<string> args)
    {
        if (!message.Chat.IsGroupChat)
        {
            throw new ControllerException("Эту команду нельзя использовать в личном чате");
        }
        Game? game = _gamesManager.GetGameByChatId(message.Chat.Id);
        if (game == null)
        {
            throw new ControllerException("В этом чате нету игры");
        }
        if (!game.Players.Select(user => user.Id).Contains(message.From.Id))
        {
            throw new ControllerException("Вы не являетесь участником игры");
        }
        MentionInfo[] mentions = message.Mentions.ToArray();
        if (mentions.Length != 2)
        {
            throw new ControllerException("Неправильное количество игроков");
        }
        if (mentions[0].Start != args[0].Length + 1)
        {
            throw new ControllerException("Неправильно указан 1-ый игрок");
        }
        if (mentions[1].Start != mentions[0].Start + mentions[0].Length + 1)
        {
            throw new ControllerException("Неправильно указан 2-ой игрок");
        }

        bot.ReplyTo(message, "Карты: 0/2");
        bot.Send(mentions[0].User.Id, $"Напишите свою карту для партии номер {game.GameNumber}");
        bot.Send(mentions[1].User.Id, $"Напишите свою карту для партии номер {game.GameNumber}");
        new PlayersMessageWaiter(
            bot, 
            new[] { mentions[0].User, mentions[1].User },
            playersToMessages =>
            {
                foreach (var playerToMessage in playersToMessages)
                {
                    bot.ReplyTo(
                        message,
                        $"{bot.CreateMention(playerToMessage.Key.Id, playerToMessage.Key.Name)} поставил {playerToMessage.Value}"
                        );
                }
            }, (message1, playersToMessages) =>
            {
                bot.ReplyTo(message1, "Карта принята");
                bot.Send(message.Chat.Id, $"Карты: {playersToMessages.Count}/2");
            }).Start();
    }
}