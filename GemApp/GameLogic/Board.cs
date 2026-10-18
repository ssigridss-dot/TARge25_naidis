using GemApp.Models;

namespace GemApp.GemLogic;

public class Board
{
    public const int Rows = 8;
    public const int Cols = 8;

    public Gem[,] Cells { get; } = new Gem[Rows, Cols];

    private readonly Random random = new();

    private static readonly GemType[] GemTypes =
        Enum.GetValues<GemType>();

    public Board()
    {
        GenerateBoard();
    }

    private Gem CreateRandomGem()
    {
        return new Gem(GemTypes[random.Next(GemTypes.Length)]);
    }

    public void GenerateBoard()
    {
        // Alguses ei tohi laual olla automaatseid kolmikuid.
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                do
                {
                    Cells[row, col] = CreateRandomGem();
                }
                while (CreatesMatchAt(row, col));
            }
        }
    }

    private bool CreatesMatchAt(int row, int col)
    {
        GemType type = Cells[row, col].Type;

        if (col >= 2 &&
            Cells[row, col - 1].Type == type &&
            Cells[row, col - 2].Type == type)
        {
            return true;
        }

        if (row >= 2 &&
            Cells[row - 1, col].Type == type &&
            Cells[row - 2, col].Type == type)
        {
            return true;
        }

        return false;
    }

    public bool AreAdjacent(
        int row1, int col1,
        int row2, int col2)
    {
        return Math.Abs(row1 - row2) +
               Math.Abs(col1 - col2) == 1;
    }

    public bool TrySwap(
        int row1, int col1,
        int row2, int col2)
    {
        if (!AreAdjacent(row1, col1, row2, col2))
            return false;

        Swap(row1, col1, row2, col2);

        // Kui vahetus ei tekitanud kolmikut,
        // taastame algse olukorra.
        if (MatchDetector.FindMatches(Cells).Count == 0)
        {
            Swap(row1, col1, row2, col2);
            return false;
        }

        return true;
    }

    private void Swap(
        int row1, int col1,
        int row2, int col2)
    {
        (Cells[row1, col1], Cells[row2, col2]) =
            (Cells[row2, col2], Cells[row1, col1]);
    }

    public int ResolveMatches(
    Action<GemType>? onGemRemoved = null)
    {
        int totalRemoved = 0;

        while (true)
        {
            var matches = MatchDetector.FindMatches(Cells);

            if (matches.Count == 0)
                break;

            foreach (var (row, col) in matches)
            {
                GemType type = Cells[row, col].Type;
                onGemRemoved?.Invoke(type);
                Cells[row, col] = null!;
            }

            totalRemoved += matches.Count;

            ApplyGravity();
            FillEmptyCells();
        }

        return totalRemoved;
    }

    private void ApplyGravity()
    {
        for (int col = 0; col < Cols; col++)
        {
            int writeRow = Rows - 1;

            for (int row = Rows - 1; row >= 0; row--)
            {
                if (Cells[row, col] != null)
                {
                    Cells[writeRow, col] = Cells[row, col];

                    if (writeRow != row)
                        Cells[row, col] = null!;

                    writeRow--;
                }
            }

            // Ülejäänud kohad jäetakse tühjaks.
            while (writeRow >= 0)
            {
                Cells[writeRow, col] = null!;
                writeRow--;
            }
        }
    }

    private void FillEmptyCells()
    {
        for (int col = 0; col < Cols; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                if (Cells[row, col] == null)
                    Cells[row, col] = CreateRandomGem();
            }
        }
    }
}