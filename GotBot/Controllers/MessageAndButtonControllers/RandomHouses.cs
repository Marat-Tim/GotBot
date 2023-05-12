using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class RandomHouses : AliasCommand
{
    private readonly IGamesManager _gamesManager;

    public RandomHouses(IGamesManager gamesManager) : base("/random_houses", "/рандомные_дома")
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
        game.SendRandomHouses(bot, update.Chat.Id);
    }
}