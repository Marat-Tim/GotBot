using BotLibrary;

namespace VkBotLibrary;

internal record VkUserInfo(long Id, string Name) : IUserInfo;