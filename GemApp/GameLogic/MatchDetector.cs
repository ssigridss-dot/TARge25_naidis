using GemApp.Models;

namespace GemApp.GemLogic;

public static class MatchDetector
{
    public static HashSet<(int Row, int Col)> FindMatches(
        Gem[,] cells)
    {
        int rows = cells.GetLength(0);
        int cols = cells.GetLength(1);

        var matches = new HashSet<(int Row, int Col)>();

        // Horisontaalsed kombinatsioonid
        for (int row = 0; row < rows; row++)
        {
            int start = 0;

            while (start < cols)
            {
                int end = start + 1;

                while (end < cols &&
                       cells[row, end].Type ==
                       cells[row, start].Type)
                {
                    end++;
                }

                if (end - start >= 3)
                {
                    for (int col = start; col < end; col++)
                    {
                        matches.Add((row, col));
                    }
                }

                start = end;
            }
        }

        // Vertikaalsed kombinatsioonid
        for (int col = 0; col < cols; col++)
        {
            int start = 0;

            while (start < rows)
            {
                int end = start + 1;

                while (end < rows &&
                       cells[end, col].Type ==
                       cells[start, col].Type)
                {
                    end++;
                }

                if (end - start >= 3)
                {
                    for (int row = start; row < end; row++)
                    {
                        matches.Add((row, col));
                    }
                }

                start = end;
            }
        }

        return matches;
    }
}