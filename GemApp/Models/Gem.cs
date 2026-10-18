
namespace GemApp.Models;

public class Gem
{
    public string Type { get; private set; }
    public string Symbol { get; private set; }

    public Gem(string type, string symbol)
    {
        Type = type;
        Symbol = symbol;
    }
}
