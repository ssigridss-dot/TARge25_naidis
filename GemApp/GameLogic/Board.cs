
using GemApp.Models;

namespace GemApp.GemLogic;

public class Board
{
    private readonly Random random = new();
    private readonly MatchDetector detector = new();

    private readonly string[] types =
    {
        "Ruby", "Emerald", "Sapphire",
        "Amethyst", "Topaz", "Diamond"
    };

    private readonly string[] symbols =
    {
        "🔴", "🟢", "🔵", "🟣", "🟡", "💎"
    };

    public int Rows { get; } = 8;
    public int Columns { get; } = 8;

    public Gem?[,] Gems { get; private set; }

    public Board()
    {
        Gems = new Gem?[Rows, Columns];
        FillBoard();
    }

    private Gem CreateRandomGem()
    {
        int index = random.Next(types.Length);
        return new Gem(types[index], symbols[index]);
    }

    public void FillBoard()
    {
        // Proovi luua mängulaud, kus pole algseid
        // kombinatsioone ja leidub vähemalt üks käik.
        for (int attempt = 0; attempt < 200; attempt++)
        {
            Gems = new Gem?[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Gem gem;

                    do
                    {
                        gem = CreateRandomGem();
                    }
                    while (WouldCreateMatch(row, col, gem.Type));

                    Gems[row, col] = gem;
                }
            }

            if (HasPossibleMove())
                return;
        }

        // Väga ebatõenäoline varuvariant.
        // Tekita uus laud ja eemalda algsed kombinatsioonid.
        Gems = new Gem?[Rows, Columns];

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Gem gem;

                do
                {
                    gem = CreateRandomGem();
                }
                while (WouldCreateMatch(row, col, gem.Type));

                Gems[row, col] = gem;
            }
        }
    }

    private bool WouldCreateMatch(int row, int col, string type)
    {
        if (col >= 2 &&
            Gems[row, col - 1]?.Type == type &&
            Gems[row, col - 2]?.Type == type)
        {
            return true;
        }

        if (row >= 2 &&
            Gems[row - 1, col]?.Type == type &&
            Gems[row - 2, col]?.Type == type)
        {
            return true;
        }

        return false;
    }

    public bool IsInside(int row, int col)
    {
        return row >= 0 && row < Rows &&
               col >= 0 && col < Columns;
    }

    public bool Swap(int row1, int col1, int row2, int col2)
    {
        if (!IsInside(row1, col1) ||
            !IsInside(row2, col2))
        {
            return false;
        }

        // Vahetada saab ainult kõrvuti asuvaid kive.
        if (Math.Abs(row1 - row2) +
            Math.Abs(col1 - col2) != 1)
        {
            return false;
        }

        (Gems[row1, col1], Gems[row2, col2]) =
            (Gems[row2, col2], Gems[row1, col1]);

        return true;
    }

    public void RemoveMatches(
        HashSet<(int Row, int Col)> matches)
    {
        foreach (var (row, col) in matches)
        {
            Gems[row, col] = null;
        }

        // Kivid kukuvad iga veeru alumisse ossa.
        for (int col = 0; col < Columns; col++)
        {
            int targetRow = Rows - 1;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (Gems[row, col] != null)
                {
                    Gems[targetRow, col] = Gems[row, col];

                    if (targetRow != row)
                        Gems[row, col] = null;

                    targetRow--;
                }
            }

            // Lisa tühjadesse kohtadesse uued kivid.
            for (int row = targetRow; row >= 0; row--)
            {
                Gems[row, col] = CreateRandomGem();
            }
        }
    }

    public bool HasPossibleMove()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                // Kontrolli vahetust paremale.
                if (col + 1 < Columns)
                {
                    Swap(row, col, row, col + 1);

                    bool found =
                        detector.FindMatches(Gems).Count > 0;

                    Swap(row, col, row, col + 1);

                    if (found)
                        return true;
                }

                // Kontrolli vahetust alla.
                if (row + 1 < Rows)
                {
                    Swap(row, col, row + 1, col);

                    bool found =
                        detector.FindMatches(Gems).Count > 0;

                    Swap(row, col, row + 1, col);

                    if (found)
                        return true;
                }
            }
        }

        return false;
    }
}
