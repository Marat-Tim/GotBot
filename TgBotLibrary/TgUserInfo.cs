using BotLibrary;

namespace TgBotLibrary;

public record TgUserInfo(long Id, string Name) : IUserInfo;