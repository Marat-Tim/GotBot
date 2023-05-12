namespace BotLibrary;

public class KeyboardBuilder
{
    private int _currentLineNumber = 0;

    private bool _isInline = false;

    private readonly List<List<Button>> _buttons = new();

    public KeyboardBuilder()
    {
        _buttons.Add(new List<Button>());
    }

    public KeyboardBuilder SetInline(bool inline = true)
    {
        _isInline = inline;
        return this;
    }

    public KeyboardBuilder Button(Button button)
    {
        _buttons[_currentLineNumber].Add(button);
        return this;
    }

    public KeyboardBuilder Button(string text, Color color = Color.Without)
    {
        return Button(new Button(text, color));
    }

    public KeyboardBuilder NewLine()
    {
        ++_currentLineNumber;
        _buttons.Add(new List<Button>());
        return this;
    }

    public Keyboard Build()
    {
        return new Keyboard(_buttons, _isInline);
    }
}