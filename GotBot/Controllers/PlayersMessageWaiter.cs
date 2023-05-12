using BotLibrary;

namespace GotBot.Controllers;

public class PlayersMessageWaiter
{
    private readonly IBot _bot;
    private readonly IEnumerable<IUserInfo> _players;
    private readonly Action<IDictionary<IUserInfo, string>> _actionWhenAwaited;
    private readonly Action<IMessage, IDictionary<IUserInfo, string>> _actionWhenReceived;

    private readonly IDictionary<IUserInfo, string> _playersToMessages = new Dictionary<IUserInfo, string>();
    private readonly ISet<long> _playerIds;

    public PlayersMessageWaiter(
        IBot bot,
        IEnumerable<IUserInfo> players,
        Action<IDictionary<IUserInfo, string>> actionWhenAwaited,
        Action<IMessage, IDictionary<IUserInfo, string>> actionWhenReceived)
    {
        _bot = bot;
        _players = players;
        _actionWhenAwaited = actionWhenAwaited;
        _actionWhenReceived = actionWhenReceived;

        _playerIds = _players.Select(player => player.Id).ToHashSet();
    }

    public void Start()
    {
        _bot.OnMessageReceived += Handler;
    }

    private void Handler(IBot bot, IMessage message)
    {
        if (_playerIds.Contains(message.From.Id) && !message.Chat.IsGroupChat)
        {
            _playersToMessages[_players.First(player => player.Id == message.From.Id)] = message.Text;
            _actionWhenReceived?.Invoke(message, _playersToMessages);
            if (_playersToMessages.Count == _playerIds.Count)
            {
                _actionWhenAwaited(_playersToMessages);
                _bot.OnMessageReceived -= Handler;
            }
        }
    }
}