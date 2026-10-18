
using GemApp.Models;

namespace GemApp.GemLogic;

public class MatchDetector
{
    public HashSet<(int Row, int Col)> FindMatches(Gem?[,] gems)
    {
        int rows = gems.GetLength(0);
        int cols = gems.GetLength(1);

        var matches = new HashSet<(int Row, int Col)>();

        // Otsi horisontaalseid kombinatsioone.
        for (int row = 0; row < rows; row++)
        {
            int col = 0;

            while (col < cols)
            {
                Gem? gem = gems[row, col];

                if (gem == null)
                {
                    col++;
                    continue;
                }

                int end = col + 1;

                while (end < cols &&
                       gems[row, end]?.Type == gem.Type)
                {
                    end++;
                }

                if (end - col >= 3)
                {
                    for (int i = col; i < end; i++)
                    {
                        matches.Add((row, i));
                    }
                }

                col = end;
            }
        }

        // Otsi vertikaalseid kombinatsioone.
        for (int col = 0; col < cols; col++)
        {
            int row = 0;

            while (row < rows)
            {
                Gem? gem = gems[row, col];

                if (gem == null)
                {
                    row++;
                    continue;
                }

                int end = row + 1;

                while (end < rows &&
                       gems[end, col]?.Type == gem.Type)
                {
                    end++;
                }

                if (end - row >= 3)
                {
                    for (int i = row; i < end; i++)
                    {
                        matches.Add((i, col));
                    }
                }

                row = end;
            }
        }

        return matches;
    }
}
