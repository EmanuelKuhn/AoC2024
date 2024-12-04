namespace AoC2024;

public class Day4() : AoCDay(day: 4, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        var grid = lines.Select(x => x.ToCharArray()).ToArray();
        
        var result = 0L;

        
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid.Length; y++)
            {
                result += checkXMAS(grid, x, y);
            }
        }

        return result;
    }

    private long checkXMAS(char[][] grid, int x, int y)
    {
        (int, int)[] orientations = [(1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (1, -1), (-1, 1), (-1, -1)];

        return orientations.Select(o => checkDiagonal("XMAS".ToCharArray(), grid, x, y, o)).Sum();
    }

    private int checkDiagonal(char[] word, char[][] grid, int x, int y, (int, int) orientation)
    {

        var oy = orientation.Item1;
        var ox = orientation.Item2;
        
        return word.Select((c, i) =>
        {
            var yIndex = y + oy * i;
            var xIndex = x + ox * i;

            if (yIndex < 0 || yIndex >= grid.Length || xIndex < 0 || xIndex >= grid[yIndex].Length)
            {
                return false;
            }
            
            return grid[yIndex][xIndex] == c;
        }).All(x => x) ? 1 : 0;
    }
    
    private long checkMASMAS(char[][] grid, int x, int y)
    {
        List<(int, int)> os = [(1, 1), (-1, -1), (1, -1), (-1, 1)];

        var word = "MAS".ToCharArray();
        throw new NotImplementedException();

        // return os.Select(o => checkDiagonal(word, grid, x, y, o)).Sum();
    }
    
    protected override long Part2(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        var grid = lines.Select(x => x.ToCharArray()).ToArray();
        
        var result = 0L;

        
        for (int x = 0; x < grid.Length; x++)
        {
            for (int y = 0; y < grid.Length; y++)
            {
                result += checkMASMAS(grid, x, y);
            }
        }

        return result;
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