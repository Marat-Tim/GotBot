using System.Text.Json;
using BotLibrary;
using GotBot.GameClasses;

namespace GotBot;

#warning Плохой класс
public class GamesManager : IGamesManager
{
    private readonly string _pathToFile;

    private readonly IDictionary<long, Game> _chatIdToGame;

    public GamesManager(string pathToFile)
    {
        _pathToFile = pathToFile;
        if (!File.Exists(_pathToFile))
        {
            File.WriteAllText(_pathToFile, "");
        }
        var text = File.ReadAllText(pathToFile);
        if (string.IsNullOrEmpty(text))
        {
            _chatIdToGame = new Dictionary<long, Game>();
        }
        else
        {
            _chatIdToGame = JsonSerializer.Deserialize<IDictionary<long, Game>>(text) ?? new Dictionary<long, Game>();
        }
    }
    public int StartGame(long chatId, IEnumerable<IUserInfo> players, int trustLevel)
    {
        int gameNumber = 1;
        while (_chatIdToGame.Values.FirstOrDefault(game => game.GameNumber == gameNumber, null) != null)
        {
            ++gameNumber;
        }
        _chatIdToGame[chatId] = new Game(
            gameNumber,
            trustLevel,
            players.Select(player => new User(player.Id, player.Name)),
            Decks.Westeros.Select(deck => deck.Shuffle()),
            Decks.Wildlings.Shuffle()
            );
        SaveChanges(chatId);
        return gameNumber;
    }

    public Game? GetGameByChatId(long chatId)
    {
        return _chatIdToGame.TryGetValue(chatId, out var value) ? value : null;
    }

    public void SaveChanges(long chatId)
    {
        File.WriteAllText(_pathToFile, JsonSerializer.Serialize(_chatIdToGame));
    }

    public void ForEachGameInvoke(Action<long, Game> action)
    {
        foreach (var chatToGame in _chatIdToGame)
        {
            action(chatToGame.Key, chatToGame.Value);
        }
    }
}