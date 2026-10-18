
using GemApp.Models;

namespace GemApp.GemLogic;

public class Game
{
    private readonly MatchDetector matchDetector = new();

    public Board Board { get; private set; }
    public Player Player { get; }
    public bool IsGameOver { get; private set; }

    public int TargetScore { get; } = 500;

    public Game()
    {
        Board = new Board();
        Player = new Player("Mängija");
    }

    public bool TrySwap(
        int row1, int col1,
        int row2, int col2)
    {
        if (IsGameOver)
            return false;

        if (!Board.Swap(row1, col1, row2, col2))
            return false;

        var matches = matchDetector.FindMatches(Board.Gems);

        if (matches.Count == 0)
        {
            // Sobimatu vahetus: taasta algne olukord.
            Board.Swap(row1, col1, row2, col2);
            return false;
        }

        Player.MakeMove();

        while (matches.Count > 0)
        {
            Player.AddPoints(matches.Count * 10);

            Board.RemoveMatches(matches);

            matches = matchDetector.FindMatches(Board.Gems);
        }

        if (Player.Score >= TargetScore)
        {
            IsGameOver = true;
        }
        else if (!Board.HasPossibleMove())
        {
            // Kui käike enam pole, loo uus mängulaud.
            Board.FillBoard();
        }

        return true;
    }

    public void Restart()
    {
        Board = new Board();
        Player.Reset();
        IsGameOver = false;
    }
}
