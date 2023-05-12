using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class RandomOrder : AliasCommand
{
    private readonly IGamesManager _gamesManager;

    public RandomOrder(IGamesManager gamesManager) : base("/random_order", "/рандомный_порядок")
    {
        _gamesManager = gamesManager;
    }

    protected override void ControlUpdate(IBot bot, IUpdate update)
    {
        if (!update.Chat.IsGroupChat)
        {
            throw new ControllerException("Эту команду нельзя применять в личном чате");
        }
        Game? game = _gamesManager.GetGameByChatId(update.Chat.Id);
        if (game == null)
        {
            throw new ControllerException("В этом чате нету игры");
        }
        game.SendRandomOrder(bot, update.Chat.Id);
    }
}