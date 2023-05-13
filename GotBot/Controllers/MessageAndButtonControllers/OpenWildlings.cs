using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class OpenWildlings : AliasCommand
{
    private readonly IGamesManager _gamesManager;

    public OpenWildlings(IGamesManager gamesManager) : base("/wildlings", "/одичалые")
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
        if (!game.Players.Select(user => user.Id).Contains(update.From.Id))
        {
            throw new ControllerException("Вы не являетесь участником игры");
        }
        new ConfirmationDialogue(
            bot,
            update.Chat.Id,
            game.Players.Select(player => player.Id),
            game.TrustLevel,
            (bot1, update1) =>
            {
                game.OpenWildlings(bot1, update.Chat.Id);
                _gamesManager.SaveChanges(update.Chat.Id);
            }, keyboard: Keyboards.GameKeyboard).Start();
    }

    
}