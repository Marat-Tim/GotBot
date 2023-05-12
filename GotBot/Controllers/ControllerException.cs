namespace GotBot.Controllers;

public class ControllerException : Exception
{
    public ControllerException(string message) : base(message) { }
}