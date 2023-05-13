#define vk1

using System.Text.Json;
using BotLibrary;
using GotBot;
using GotBot.Controllers.MessageAndButtonControllers;
using GotBot.Controllers.MessageControllers;
using TgBotLibrary;
using VkBotLibrary;

IBot bot;

IGamesManager gamesManager;

#if vk

bot = new VkBot(Config.VkAccessToken, Config.VkGroupUrl);

gamesManager = new GamesManager("games_vk.json");

#else

bot = new TgBot(Config.TelegramToken);

gamesManager = new GamesManager("games_tg.json");

#endif

bot = new ExceptionsBotDecorator(new TgBot(Config.TelegramToken));



gamesManager.ForEachGameInvoke((chatId, game) =>
{
    try
    {
        bot.Send(chatId, "Бот начал работать", keyboard: Keyboards.GameKeyboard);
    }
    catch (BotException e) { }
});

IMessageController[] messageControllers =
{
    new StartGame(gamesManager),
    new Battle(gamesManager)
};
IMessageAndButtonController[] messageAndButtonControllers =
{
    new RandomHouses(gamesManager),
    new RandomOrder(gamesManager),
    new OpenWesteros(gamesManager),
    new OpenWildlings(gamesManager),
    new Orders(gamesManager),
    new Crow(gamesManager)
};

foreach (var messageAndButtonController in messageAndButtonControllers)
{
    bot.OnButtonClicked += messageAndButtonController.Control;
    bot.OnMessageReceived += messageAndButtonController.Control;
}

foreach (var messageController in messageControllers)
{
    bot.OnMessageReceived += messageController.Control;
}

bot.OnMessageReceived += (bot1, message) =>
{
    Console.WriteLine(JsonSerializer.Serialize(message, typeof(object), new JsonSerializerOptions() { WriteIndented = true }));
};

bot.OnButtonClicked += (bot1, message) =>
{
    Console.WriteLine(JsonSerializer.Serialize(message, typeof(object), new JsonSerializerOptions() { WriteIndented = true }));
};

Console.WriteLine("Бот начал работу");

bot.Start();
//start:
//try
//{
//    bot.Start();
//}
//catch (Exception e)
//{
//    Console.WriteLine(e.Message);
//    Thread.Sleep(10000);
//    goto start;

//}