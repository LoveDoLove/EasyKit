namespace EasyKit.Models;

public class ContextMenuAction
{
    public ContextMenuAction(string name, string command)
    {
        Name = name;
        Command = command;
    }

    public string Name { get; set; }
    public string Command { get; set; }
}