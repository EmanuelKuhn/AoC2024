using System.Collections.Frozen;
using System.Diagnostics;

namespace AoC2024;

public class Day12() : AoCDay(day: 12, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var polygons = ExtractPolygons(input);
        
        return polygons
            .Where(p => p.Type != null)
            .Select(p => p.Cells.Count * p.BoundaryEdges.Count)
            .Sum();
    }

    protected override long Part2(string input)
    {
        var polygons = ExtractPolygons(input);

        return polygons
            .Where(p => p.Type != null)
            .Select(p => CountSides(p.BoundaryEdges) * p.Cells.Count)
            .Sum();
    }

    private static List<Poly> ExtractPolygons(string input)
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

            var newPolygon = new Poly([], [], grid[next.R][next.C]);
            
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
                
                var neighboursNotInPoly = grid.Neighbours(current, (otherCell, offset) =>
                    {
                        var edge = offset switch
                        {
                            (0, 1) => new Edge(new Vec2(current.R, current.C + 1), new Vec2(current.R + 1, current.C + 1)),
                            (1, 0) => new Edge(new Vec2(current.R + 1, current.C + 1), new Vec2(current.R + 1, current.C)),
                            (0, -1) => new Edge(new Vec2(current.R + 1, current.C), new Vec2(current.R, current.C)),
                            (-1, 0) => new Edge(new Vec2(current.R, current.C), new Vec2(current.R, current.C + 1)),
                            _ => throw new ArgumentOutOfRangeException(nameof(offset), offset, null)
                        };
                        
                        return (otherCell, edge);
                    })
                    .Where(t => grid[t.otherCell.R][t.otherCell.C] != newPolygon.Type)
                    .Select(t => t.edge)
                    .ToList();
                
                neighboursNotInPoly.ForEach(e => newPolygon.BoundaryEdges.Add(e));
            }

            if (newPolygon.Type != null)
            {
                Debug.WriteLine(newPolygon.Type);

                Debug.WriteLine("Cells");
                Debug.WriteLine(string.Join("\n", newPolygon.Cells));
                Debug.WriteLine("Edges");
                Debug.WriteLine($"{string.Join("\n", newPolygon.BoundaryEdges.OrderBy(e => e.From.R).ThenBy(e => e.From.C).Select(e => $"({e.From.R}, {e.From.C}) -> ({e.To.R}, {e.To.C})"))}");
                Debug.WriteLine("end\n");
                ;
            }
            
            polygons.Add(newPolygon);
        }

        return polygons;
    }

    private static int CountSides(HashSet<Edge> boundaryEdges)
    {
        var loops = ExtractLoops(boundaryEdges);

        List<int> sidesList = new List<int>();
        
        foreach (var loop in loops)
        {
            var sides = 0;
            
            for (int i = 1; i <= loop.Count; i++)
            {
                var prev = loop[(i - 1) % loop.Count];
                var cur = loop[i % loop.Count];
                var next = loop[(i + 1) % loop.Count];

                if (next - cur != cur - prev)
                {
                    sides += 1;
                }
            }
            
            sidesList.Add(sides);
        }

        return sidesList.Sum();
    }

    private static HashSet<List<Vec2>> ExtractLoops(HashSet<Edge> boundaryEdges)
    {
        boundaryEdges = boundaryEdges.ToHashSet();

        var loops = new HashSet<List<Vec2>>();

        Debug.WriteLine($"{string.Join("\n", boundaryEdges.OrderBy(e => e.From.R).ThenBy(e => e.From.C).Select(e => $"({e.From.R}, {e.From.C}) -> ({e.To.R}, {e.To.C})"))}");
        
        while (boundaryEdges.Count > 0)
        {
            Debug.WriteLine("new loop");
            var edge = boundaryEdges.First();
            boundaryEdges.Remove(edge);

            var loop = new List<Vec2>
            {
                edge.From,
            };
            

            while (true)
            {
                Debug.WriteLine($"({edge.From.R}, {edge.From.C}) -> ({edge.To.R}, {edge.To.C})");
                edge = boundaryEdges.FirstOrDefault(e => e.From == edge.To);

                if (edge == default) break;

                boundaryEdges.Remove(edge);
                loop.Add(edge.From);
            }

            loops.Add(loop);
        }

        return loops;
    }

    private static char[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(l => l.Select(c => c).ToArray()).ToArray();
    }
    

    [Test]
    [TestCase(1, 1930)]
    [TestCase(2, 1206)]
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
record struct Poly(HashSet<Vec2> Cells, HashSet<Edge> BoundaryEdges, char? Type);
}


internal readonly record struct Edge(Vec2 From, Vec2 To);

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

    static readonly (int, int)[] Offsets = [(0, 1), (0, -1), (1, 0), (-1, 0)];
    
    public static IEnumerable<Vec2> Neighbours<T>(this T[][] grid, Vec2 position)
    {
        var coords = Offsets.Select(o => new Vec2(position.R + o.Item1, position.C + o.Item2)).ToArray();

        return coords.Where(grid.IsInGrid);
    }
    
    public static IEnumerable<TOut> Neighbours<T, TOut>(this T[][] grid, Vec2 position, Func<Vec2, (int r, int c), TOut> selector)
    {
        var r = Offsets.Select(o => (new Vec2(position.R + o.Item1, position.C + o.Item2), o))
            .Where(t => grid.IsInGrid(t.Item1))
            .ToArray();
        return r.Select(t => selector(t.Item1, t.Item2));
    }
}
