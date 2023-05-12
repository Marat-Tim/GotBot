using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class OpenWesteros : AliasCommand
{
    private readonly IGamesManager _gameManager;

    public OpenWesteros(IGamesManager gamesManager) : base("/westeros", "/вестерос")
    {
        _gameManager = gamesManager;
    }

    protected override void ControlUpdate(IBot bot, IUpdate update)
    {
        if (!update.Chat.IsGroupChat)
        {
            throw new ControllerException("Эту команду нельзя применять в групповом чате");
        }
        Game? game = _gameManager.GetGameByChatId(update.Chat.Id);
        if (game == null)
        {
            throw new ControllerException("В этом чате нет игры");
        }
        new ConfirmationDialogue(
            bot, 
            update.Chat.Id,
            game.Players.Select(player => player.Id),
            game.TrustLevel,
            (bot1, update1) =>
            {
                game.OpenWesteros(bot1, update1.Chat.Id);
                _gameManager.SaveChanges(update1.Chat.Id);
            }, keyboard: Keyboards.GameKeyboard).Start();
    }
}