using BotLibrary;

namespace TgBotLibrary;

public record TgChatInfo(long Id, bool IsGroupChat) : IChatInfo;