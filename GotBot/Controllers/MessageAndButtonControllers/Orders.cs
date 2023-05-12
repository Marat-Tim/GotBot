using BotLibrary;
using GotBot.GameClasses;

namespace GotBot.Controllers.MessageAndButtonControllers;

public class Orders : AliasCommand
{
    private readonly IGamesManager _gameManager;

    public Orders(IGamesManager gameManager) : base("/orders", "/приказы", "/ставки", "/bids")
    {
        _gameManager = gameManager;
    }

    protected override void ControlUpdate(IBot bot, IUpdate update)
    {
        if (!update.Chat.IsGroupChat)
        {
            throw new ControllerException("Эту команду нельзя использовать в групповом чате");
        }
        Game? game = _gameManager.GetGameByChatId(update.Chat.Id);
        if (game == null)
        {
            throw new ControllerException("В этом чате нету игры");
        }
        bot.ReplyTo(update, $"0/{game.Players.Count()}");
        new OrdersWaiter(bot, update.Chat.Id, game).Start();
    }

    private class OrdersWaiter
    {
        private readonly Game _game;
        private readonly long _chatId;
        private readonly IBot _bot;

        private readonly IDictionary<long, string> _playerIdToOrders;
        private readonly ISet<long> _playerIds;

        public OrdersWaiter(IBot bot, long chatId, Game game)
        {
            _bot = bot;
            _chatId = chatId;
            _game = game;
            _playerIdToOrders = new Dictionary<long, string>();
            _playerIds = _game.Players.Select(player => player.Id).ToHashSet();
        }

        public void Start()
        {
            foreach (var playerId in _playerIds)
            {
                _bot.Send(playerId,
                    $"Напиши свои приказы или ставку для партии номер {_game.GameNumber}");
            }
            _bot.OnMessageReceived += Handler;
        }

        private void Handler(IBot bot, IMessage message)
        {
            if (_playerIds.Contains(message.From.Id) && !message.Chat.IsGroupChat)
            {
                _playerIdToOrders[message.From.Id] = message.Text;
                bot.ReplyTo(message, "Принято");
                bot.Send(_chatId, $"{_playerIdToOrders.Count}/{_playerIds.Count}");
                if (_playerIdToOrders.Count == _playerIds.Count)
                {
                    foreach (var playerIdToOrder in _playerIdToOrders)
                    {
                        string name = _game.Players.First(player => player.Id == playerIdToOrder.Key).Name;
                        bot.Send(_chatId, $"{_bot.CreateMention(playerIdToOrder.Key, name)} поставил:\n{playerIdToOrder.Value}");
                    }
                    _bot.OnMessageReceived -= Handler;
                }
            }
        }
    }
}