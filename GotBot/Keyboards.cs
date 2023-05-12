using BotLibrary;

namespace GotBot;

public static class Keyboards
{
    public static readonly Keyboard ConfirmOrCancelKeyboard = new KeyboardBuilder()
        .Button("/подтвердить", Color.Positive)
        .Button("/отмена", Color.Negative)
        .Build();

    public static readonly Keyboard GameKeyboard = new KeyboardBuilder()
        .Button("/вестерос").Button("/одичалые").NewLine()
        .Button("/приказы").Button("/ставки").Button("/ворона")
        .Build();
}