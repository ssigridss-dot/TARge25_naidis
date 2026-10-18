namespace GemApp.Models;

public enum GemType
{
    Blue,
    Green,
    Red,
    Yellow,
    Orange,
    Purple,
    Black
}

public class Gem
{
    public GemType Type { get; set; }

    public Gem(GemType type)
    {
        Type = type;
    }
}