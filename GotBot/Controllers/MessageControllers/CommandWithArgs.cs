using BotLibrary;

namespace GotBot.Controllers.MessageControllers;

public abstract class CommandWithArgs : IMessageController
{
    private readonly int? _argumentCount;

    private readonly IReadOnlyList<string> _aliases;

    protected CommandWithArgs(int argumentCount, string firstAlias, params string[] aliases) : this(firstAlias, aliases)
    {
        _argumentCount = argumentCount;
    }

    protected CommandWithArgs(string firstAlias, params string[] aliases)
    {
        _aliases = aliases.Concat(new[] { firstAlias }).ToList();
    }

    public void Control(IBot bot, IMessage message)
    {
        foreach (var alias in _aliases)
        {
            if (message.Text.StartsWith(alias))
            {
                var args = message.Text.Split(' ');
                if (_argumentCount.HasValue && _argumentCount == args.Length || !_argumentCount.HasValue)
                {
                    Control(bot, message, args);
                    return;
                }
                else
                {
                    throw new ControllerException("Неправильное количество аргументов");
                }
            }
        }
    }

    protected abstract void Control(IBot bot, IMessage message, IReadOnlyList<string> args);
}