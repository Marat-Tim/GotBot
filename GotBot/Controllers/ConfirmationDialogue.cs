using BotLibrary;

namespace GotBot.Controllers;

public class ConfirmationDialogue
{
    private static readonly ISet<long> ChatsWithConfirmationDialogue = new HashSet<long>();

    private readonly IBot _bot;

    private readonly long _chatId;

    private readonly int _requiredConfirmationCount;

    private readonly ISet<long> _participantIds;

    private readonly Action<IBot, IUpdate> _actionWhenConfirming;

    private readonly Action<IBot, IUpdate>? _actionIfNotConfirm;

    private readonly Keyboard? _keyboard;

    private int _currentConfirmationCount;

    public ConfirmationDialogue(
        IBot bot,
        long chatId,
        IEnumerable<long> participantIds,
        int requiredConfirmationCount,
        Action<IBot, IUpdate> actionWhenConfirming,
        Action<IBot, IUpdate>? actionIfNotConfirm = null,
        Keyboard? keyboard = null
        )
    {
        _bot = bot;
        _currentConfirmationCount = 0;
        _chatId = chatId;
        _requiredConfirmationCount = requiredConfirmationCount;
        _actionWhenConfirming = actionWhenConfirming;
        _actionIfNotConfirm = actionIfNotConfirm;
        _keyboard = keyboard;
        _participantIds = participantIds.ToHashSet();
    }

    private void Control(IBot bot, IUpdate update)
    {
        if (update.Chat.Id == _chatId && _participantIds.Contains(update.From.Id))
        {
            if (update.Text == "/подтвердить" || update.Text == "/confirm")
            {
                ++_currentConfirmationCount;
                _participantIds.ExceptWith(new[] { update.From.Id });
                bot.ReplyTo(update, $"Подтверждений: {_currentConfirmationCount}/{_requiredConfirmationCount}");
                if (_currentConfirmationCount == _requiredConfirmationCount)
                {
                    if (_keyboard == null)
                    {
                        bot.DeleteKeyboard(_chatId, "Действие подтверждено");
                    }
                    else
                    {
                        bot.Send(_chatId, "Действие подтверждено", _keyboard);
                    }
                    _actionWhenConfirming?.Invoke(bot, update);
                    bot.OnButtonClicked -= Control;
                    bot.OnMessageReceived -= Control;
                    ChatsWithConfirmationDialogue.ExceptWith(new[] { _chatId });
                }
            }
            if (update.Text == "/отменить" || update.Text == "/cancel")
            {
                if (_keyboard == null)
                {
                    bot.DeleteKeyboard(_chatId, "Действие не подтверждено");
                }
                else
                {
                    bot.Send(_chatId, "Действие не подтверждено", _keyboard);
                }
                bot.OnButtonClicked -= Control;
                bot.OnMessageReceived -= Control;
                ChatsWithConfirmationDialogue.ExceptWith(new[] { _chatId });
                _actionIfNotConfirm?.Invoke(bot, update);
            }
        }
    }

    public void Start()
    {
        if (ChatsWithConfirmationDialogue.Contains(_chatId))
        {
            _bot.Send(_chatId, "Подтвердите или отмените старое действие прежде чем делать новое");
        }
        else
        {
            _bot.Send(
                _chatId,
                $"Это действие требует {_requiredConfirmationCount} подтверждений",
                Keyboards.ConfirmOrCancelKeyboard);
            _bot.OnButtonClicked += Control;
            _bot.OnMessageReceived += Control;
            ChatsWithConfirmationDialogue.Add(_chatId);
        }
    }
}