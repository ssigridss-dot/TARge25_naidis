
using System;
using System.Collections.Generic;
using System.Linq;
using GemApp.Models;

namespace GemApp.GemLogic;

public class Game
{
    public Board Board { get; private set; }

    public int Score { get; private set; }

    public int TargetScore { get; set; } = 500;

    public int Moves { get; private set; }

    public bool GameOver { get; private set; }

    public Dictionary<GemType, int> CollectedGems { get; }
        = Enum.GetValues<GemType>()
            .ToDictionary(type => type, _ => 0);

    public Game()
    {
        Board = new Board();
    }

    public bool MakeMove(
        int row1, int col1,
        int row2, int col2)
    {
        if (GameOver)
            return false;

        if (!Board.TrySwap(row1, col1, row2, col2))
            return false;

        Moves++;

        int removed = Board.ResolveMatches(type =>
        {
            RecordCollectedGem(type);
        });

        Score += removed * 60;

        return true;
    }

    public void EndGame()
    {
        GameOver = true;
    }

    public void Restart()
    {
        Board = new Board();

        Score = 0;
        Moves = 0;
        GameOver = false;

        foreach (GemType type in CollectedGems.Keys.ToList())
        {
            CollectedGems[type] = 0;
        }
    }

    public void RecordCollectedGem(GemType type)
    {
        CollectedGems[type]++;
    }
}
