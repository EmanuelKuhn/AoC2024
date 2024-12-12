using System.Collections.Frozen;
using System.Collections.Immutable;

namespace AoC2024;

public class Day12() : AoCDay(day: 12, hasTwoInputs: false)
{
    private record struct Poly(HashSet<Vec2> Cells, int Boundary, char? Type);
    
    protected override long Part1(string input)
    {
        var parsedGrid = ParseInput(input);

        var grid = new char?[parsedGrid.Length + 2][];

        for (int row = 0; row < grid.Length; row++)
        {
            grid[row] = new char?[parsedGrid[0].Length + 2];
        }

        for (var row = 0; row < parsedGrid.Length; row++)
        {
            for (int col = 0; col < parsedGrid[0].Length; col++)
            {
                grid[row + 1][col + 1] = parsedGrid[row][col];
            }
        }

        var allPosition = grid.Positions().ToFrozenSet();

        var polygons = new List<Poly>();
        
        var seen = new HashSet<Vec2>();

        while (allPosition.Except(seen).Any())
        {
            var queue = new Queue<Vec2>();
            var next = allPosition.Except(seen).First();
            queue.Enqueue(next);
            seen.Add(next);

            var newPolygon = new Poly([], 0, grid[next.R][next.C]);
            
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                newPolygon.Cells.Add(current);

                var neighboursInPoly = grid.Neighbours(current)
                    .Where(n => !seen.Contains(n))
                    .Where(n => grid[n.R][n.C] == newPolygon.Type)
                    .ToList();
                neighboursInPoly.ForEach(n =>
                {
                    queue.Enqueue(n);
                    seen.Add(n);
                });
                
                var neighboursNotInPoly = grid.Neighbours(current)
                    .Where(n => grid[n.R][n.C] != newPolygon.Type)
                    .ToList();
                
                newPolygon.Boundary += neighboursNotInPoly.Count;
            }

            if (newPolygon.Type != null)
            {
                // Console.WriteLine(newPolygon);
            }
            
            polygons.Add(newPolygon);
        }
        
        return polygons
            .Where(p => p.Type != null)
            .Select(p => p.Cells.Count * p.Boundary)
            .Sum();
    }

    protected override long Part2(string input)
    {
        return 0;
    }

    private static char[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(l => l.Select(c => c).ToArray()).ToArray();
    }
    

    [Test]
    [TestCase(1, 772)]
    [TestCase(2, 81)]
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

file static class GridExtensions {
    public static IEnumerable<Vec2> Positions<T>(this T[][] grid) {
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                yield return new Vec2(r, c);
            }
        }
    }
    
    public static bool IsInGrid<T>(this T[][] grid, Vec2 position)
    {
        return position is { C: >= 0, R: >= 0 } && position.R < grid.Length && position.C < grid[position.R].Length;
    }

    public static IEnumerable<Vec2> Neighbours<T>(this T[][] grid, Vec2 position)
    {
        (int, int)[] offsets = [(0, 1), (0, -1), (1, 0), (-1, 0)];
        var coords = offsets.Select(o => new Vec2(position.R + o.Item1, position.C + o.Item2)).ToArray();

        return coords.Where(grid.IsInGrid);
    }
}
