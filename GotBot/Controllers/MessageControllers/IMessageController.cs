using BotLibrary;

namespace GotBot.Controllers.MessageControllers;

public interface IMessageController
{
    void Control(IBot bot, IMessage message);
}