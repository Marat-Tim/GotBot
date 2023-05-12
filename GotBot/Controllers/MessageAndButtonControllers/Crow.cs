using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class Crow : AliasCommand
{
    private readonly IGamesManager _gamesManager;

    public Crow(IGamesManager gamesManager) : base("/crow", "/ворона")
    {
        _gamesManager = gamesManager;
    }

    protected override void ControlUpdate(IBot bot, IUpdate update)
    {
        if (!update.Chat.IsGroupChat)
        {
            throw new ControllerException("Эту команду нельзя использовать в личном чате");
        }
        Game? game = _gamesManager.GetGameByChatId(update.Chat.Id);
        if (game == null)
        {
            throw new ControllerException("В этом чате нету игры");
        }
        new ConfirmationDialogue(bot, update.Chat.Id, game.Players.Select(player => player.Id), game.TrustLevel,
            (bot1, update1) =>
            {
                game.ShowWildlings(bot, update.From.Id);
                bot1.Send(update.From.Id, "Вы хотите закопать карту?");
                new ConfirmationDialogue(bot, update.From.Id, new[] { update.From.Id }, 1,
                    (bot2, update2) =>
                {
                    game.BuryWildlings();
                    _gamesManager.SaveChanges(update.Chat.Id);
                    bot2.Send(update.Chat.Id, "Карта одичалых была закопана");
                }, (bot2, update2) =>
                    {
                        bot2.Send(update.Chat.Id, "Карта одичалых оставлена наверху");
                    }).Start();
            }, keyboard: Keyboards.GameKeyboard).Start();
    }
}