using BotLibrary;
using GotBot;
using GotBot.Controllers.MessageAndButtonControllers;
using GotBot.Controllers.MessageControllers;
using VkBotLibrary;


IBot bot = new ExceptionsBotDecorator(new VkBot(Config.VkAccessToken, Config.VkGroupUrl));


IGamesManager gamesManager = new GamesManager("games.json");

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

Console.WriteLine("Бот начал работу");

start:
try
{
    bot.Start();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Thread.Sleep(10000);
    goto start;

}