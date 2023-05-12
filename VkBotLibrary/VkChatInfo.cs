using BotLibrary;

namespace VkBotLibrary;

internal record VkChatInfo(long Id) : IChatInfo
{
    public bool IsGroupChat => Id > 2000000000; // Число взято из VkApi
}