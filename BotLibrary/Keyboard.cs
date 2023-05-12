namespace BotLibrary;

public record Keyboard(IReadOnlyCollection<IReadOnlyCollection<Button>> Buttons, bool IsInline);