using BotLibrary;
using TgBotLibrary;
using VkBotLibrary;

namespace GotBot;

public static class UserInterface
{
    public static IBot GetBotConfigFromUser()
    {
        Console.WriteLine(@"Выберите мессенджер, для которого должна работать программа:
1. VK
2. Telegram");
        int command = GetCommand(1, 2);
        if (command == 2)
        {
            Console.Write("Введите Token для Telegram бота\n> ");
            var token = Console.ReadLine();
            if (token == null)
            {
                Environment.Exit(0);
            }
            Console.Clear();
            return new TgBot(token);
        }

        if (command == 1)
        {
            Console.Write("Введите AccessToken для ВК бота\n> ");
            var accessToken = Console.ReadLine();
            if (accessToken == null)
            {
                Environment.Exit(0);
            }
            Console.Write("Введите Url вашей группы\n> ");
            var groupUrl = Console.ReadLine();
            if (groupUrl == null)
            {
                Environment.Exit(0);
            }
            Console.Clear();
            return new VkBot(accessToken, groupUrl);
        }

        throw new Exception("Пользователь ввел неизвестную команду");
    }

    static int GetCommand(int min, int max)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        int command;
        while (!(int.TryParse(input, out command) && command >= min && command <= max))
        {
            if (input == null)
            {
                Environment.Exit(0);
            }
            Console.WriteLine($"Неправильный формат входных данных. Введите число от {min} до {max}");
            input = Console.ReadLine();
        }
        return command;
    }

}