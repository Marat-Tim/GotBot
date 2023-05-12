using System.Text;
using BotLibrary;
using GotBot.Controllers;
using GotBot.Properties;

namespace GotBot.GameClasses;

public class Game
{
    private readonly IList<Queue<EventCard>> _westerosDecks;

    private readonly Queue<EventCard> _wildlingsDeck;

    public Game(
        int gameNumber,
        int trustLevel,
        IEnumerable<User> players,
        IEnumerable<IEnumerable<EventCard>> westerosDecks,
        IEnumerable<EventCard> wildlingsDeck)
    {
        GameNumber = gameNumber;
        TrustLevel = trustLevel;
        Players = players;
        this._westerosDecks = westerosDecks.Select(deck => new Queue<EventCard>(deck)).ToList();
        this._wildlingsDeck = new Queue<EventCard>(wildlingsDeck);
    }

    public int GameNumber { get; }

    public int TrustLevel { get; }

    public IEnumerable<User> Players { get; }

    public IEnumerable<IEnumerable<EventCard>> WesterosDecks => _westerosDecks;

    public IEnumerable<EventCard> WildlingsDeck => _wildlingsDeck.ToArray();

    public void SendRandomHouses(IBot bot, long chatId)
    {
        var houses = new[] { "Ланнистер", "Баратеон", "Старк", "Мартелл", "Тирелл", "Грейджой" };
        var shuffleHouses = houses.Shuffle().ToArray();
        StringBuilder replyBuilder = new StringBuilder();
        replyBuilder.Append("Дома распределены:\n");
        int i = 0;
        foreach (var player in Players)
        {
            replyBuilder.Append($"{bot.CreateMention(player.Id, player.Name)} - {shuffleHouses[i]}\n");
            ++i;
        }
        bot.Send(chatId, replyBuilder.ToString());
    }

    public void SendRandomOrder(IBot bot, long chatId)
    {
        var shufflePlayers = Players.Shuffle().ToArray();
        StringBuilder replyBuilder = new StringBuilder();
        replyBuilder.Append("Рандомный порядок:\n");
        for (int i = 0; i < shufflePlayers.Length; i++)
        {
            replyBuilder.Append($"{i + 1}. {bot.CreateMention(shufflePlayers[i].Id, shufflePlayers[i].Name)}\n");
        }
        bot.Send(chatId, replyBuilder.ToString());
    }

    public void OpenWesteros(IBot bot, long chatId)
    {
        StringBuilder replyBuilder = new StringBuilder();
        replyBuilder.Append("Карты вестероса:\n");
        List<EventCard> eventCards = new List<EventCard>();
        for (int i = 0; i < _westerosDecks.Count; i++)
        {
            replyBuilder.Append($"{i + 1}. ");
            if (_westerosDecks[i].Count == 0)
            {
                throw new ControllerException("Колода вестероса пустая");
            }
            EventCard eventCard = _westerosDecks[i].Dequeue();
            replyBuilder.Append(eventCard.Name);
            while (eventCard == Cards.WinterIsComing)
            {
                _westerosDecks[i] = new Queue<EventCard>(Decks.Westeros[i].Shuffle());
                eventCard = _westerosDecks[i].Dequeue();
                replyBuilder.Append($" -> {eventCard.Name}");
            }
            eventCards.Add(eventCard);
            replyBuilder.Append("\n");
        }
        bot.Send(
            chatId,
            replyBuilder.ToString(),
            images:
            eventCards
                .Select(
                    card => new Image(
                            (byte[])Resources.ResourceManager.GetObject(card.ResourceName)!,
                            card.Extension)
                    )
                .ToArray());
    }

    public void OpenWildlings(IBot bot, long chatId)
    {
        EventCard eventCard = _wildlingsDeck.Dequeue();
        bot.Send(chatId, $"Карта одичалых: {eventCard.Name}",
            images:
            new[]
            {
                new Image(
                (byte[])Resources.ResourceManager.GetObject(eventCard.ResourceName)!,
                eventCard.Extension)
            });
        _wildlingsDeck.Enqueue(eventCard);
    }

    public void ShowWildlings(IBot bot, long chatId)
    {
        EventCard eventCard = _wildlingsDeck.Peek();
        bot.Send(chatId,
            $"Верхняя карта колоды одичалых в партии номер {GameNumber}: {eventCard.Name}",
            images:
            new[]
            {
                new Image(
                    (byte[])Resources.ResourceManager.GetObject(eventCard.ResourceName)!,
                    eventCard.Extension)
            });
    }

    public void BuryWildlings()
    {
        _wildlingsDeck.Enqueue(_wildlingsDeck.Dequeue());
    }
}

