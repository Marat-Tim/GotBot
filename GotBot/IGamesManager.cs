using BotLibrary;
using GotBot.GameClasses;

namespace GotBot;

public interface IGamesManager
{
    int StartGame(long chatId, IEnumerable<IUserInfo> players, int trustLevel);

    Game? GetGameByChatId(long chatId);

    void SaveChanges(long chatId);

    void ForEachGameInvoke(Action<long, Game> action);

}