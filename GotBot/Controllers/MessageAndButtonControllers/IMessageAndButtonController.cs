using BotLibrary;

namespace GotBot.Controllers.MessageAndButtonControllers;

public interface IMessageAndButtonController
{
    void Control(IBot bot, IUpdate update);
}