
namespace GemApp.Models;

public class Player
{
    public string Name { get; private set; }
    public int Score { get; private set; }
    public int Moves { get; private set; }

    public Player(string name)
    {
        Name = name;
        Score = 0;
        Moves = 0;
    }

    public void AddPoints(int points)
    {
        if (points > 0)
        {
            Score += points;
        }
    }

    public void MakeMove()
    {
        Moves++;
    }

    public void Reset()
    {
        Score = 0;
        Moves = 0;
    }
}
