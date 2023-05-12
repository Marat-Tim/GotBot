using BotLibrary;

namespace GotBot.Controllers.MessageAndButtonControllers;

public abstract class AliasCommand : IMessageAndButtonController
{
    private readonly IReadOnlyList<string> _aliases;

    protected AliasCommand(string firstAlias, params string[] aliases)
    {
        _aliases = aliases.Concat(new[] { firstAlias }).ToList();
    }

    public void Control(IBot bot, IUpdate update)
    {
        foreach (var alias in _aliases)
        {
            if (update.Text == alias)
            {
                ControlUpdate(bot, update);
                return;
            }
        }
    }

    protected abstract void ControlUpdate(IBot bot, IUpdate update);
}