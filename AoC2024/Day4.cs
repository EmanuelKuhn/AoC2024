using System.Diagnostics;

namespace AoC2024;

public class Day4() : AoCDay(day: 4, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var grid = ParseGrid(input);

        var result = 0L;

        for (var y = 0; y < grid.Length; y++) 
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                result += CheckXmas(grid, x, y);
            }
        }

        return result;
    }

    private static char[][] ParseGrid(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        var grid = lines.Select(x => x.ToCharArray()).ToArray();

        Trace.Assert(grid.Length == grid[0].Length);
        
        return grid;
    }

    private static readonly (int y, int x)[] Orientations = 
        [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1)];
    
    private static long CheckXmas(char[][] grid, int x, int y)
    {
        return Orientations.Count(o => CheckDiagonal("XMAS".ToCharArray(), grid, x, y, o.y, o.x));
    }

    private static bool CheckDiagonal(char[] word, char[][] grid, int x, int y, int oy, int ox)
    {
        for (var i = 0; i < word.Length; i++)
        {
            var yIndex = y + oy * i;
            var xIndex = x + ox * i;

            if (yIndex < 0 || yIndex >= grid.Length || xIndex < 0 || xIndex >= grid[yIndex].Length)
            {
                return false;
            }

            if (grid[yIndex][xIndex] != word[i])
            {
                return false;
            }
        }

        return true;
    }
    
    protected override long Part2(string input)
    {
        var grid = ParseGrid(input);
        
        var result = 0L;
        
        for (var y = 0; y < grid.Length; y++)
        {
            for (var x = 0; x < grid[y].Length; x++)
            {
                result += CheckMasmas(grid, x, y) ? 1 : 0;
            }
        }

        return result;
    }
    
    private static bool CheckMasmas(char[][] grid, int x, int y)
    {
        var word = "MAS".ToCharArray();

        if (grid[y][x] != 'A')
        {
            return false;
        }

        var d1 = CheckDiagonal(word, grid, x - 1, y - 1, 1, 1) || CheckDiagonal(word, grid, x + 1, y + 1, -1, -1);
        var d2 = CheckDiagonal(word, grid, x - 1, y + 1, -1, 1) || CheckDiagonal(word, grid, x + 1, y - 1, 1, -1);
        
        var isXMas = d1 && d2;

        return isXMas;
    }
    
    [Test]
    [TestCase(1, 18)]
    [TestCase(2, 9)]
    public void Example(int part, long expected)
    {
        var result = part switch
        {
            1 => Part1(ExamplePart1),
            2 => Part2(ExamplePart2),
            _ => throw new ArgumentOutOfRangeException(nameof(part)),
        };
        
        Assert.That(result, Is.EqualTo(expected));
    }
}