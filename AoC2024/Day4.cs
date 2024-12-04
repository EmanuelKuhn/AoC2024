using System.Diagnostics;

namespace AoC2024;

public class Day4() : AoCDay(day: 4, hasTwoInputs: false)
{
    private const char NewLine = '\n';
    
    protected override long Part1(string input)
    {
        var grid = ParseGrid(input);

        var result = 0L;

        
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid.Length; y++)
            {
                Console.WriteLine((x, y));
                result += checkXMAS(grid, x, y);
            }
        }

        return result;
    }

    private static char[][] ParseGrid(string input)
    {
        var lines = input.Split(NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        var grid = lines.Select(x => x.ToCharArray()).ToArray();

        Trace.Assert(grid.Length == grid[0].Length);
        
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        
        Console.WriteLine((grid.Length, grid[0].Length));
        return grid;
    }

    private long checkXMAS(char[][] grid, int x, int y)
    {
        (int, int)[] orientations = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1)];

        return orientations.Count(o => checkDiagonal("XMAS".ToCharArray(), grid, x, y, o.Item1, o.Item2));
    }

    private bool checkDiagonal(char[] word, char[][] grid, int x, int y, int oy, int ox)
    {
        return word.Select((c, i) =>
        {
            var yIndex = y + oy * i;
            var xIndex = x + ox * i;

            if (yIndex < 0 || yIndex >= grid.Length || xIndex < 0 || xIndex >= grid[yIndex].Length)
            {
                return false;
            }

            var gridCel = grid[yIndex][xIndex];
            
            return gridCel == c;
        }).All(b => b);
    }
    
    protected override long Part2(string input)
    {
        var grid = ParseGrid(input);
        
        var result = 0L;
        
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid.Length; y++)
            {
                result += checkMASMAS(grid, x, y) ? 1 : 0;
            }
        }

        return result;
    }
    
    private bool checkMASMAS(char[][] grid, int x, int y)
    {
        var word = "MAS".ToCharArray();

        if (grid[y][x] != 'A')
        {
            return false;
        }

        var d1 = checkDiagonal(word, grid, x - 1, y - 1, 1, 1) || checkDiagonal(word, grid, x + 1, y + 1, -1, -1);
        var d2 = checkDiagonal(word, grid, x - 1, y + 1, -1, 1) || checkDiagonal(word, grid, x + 1, y - 1, 1, -1);
        
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