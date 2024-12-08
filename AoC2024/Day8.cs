using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace AoC2024;

public class Day8() : AoCDay(day: 8, hasTwoInputs: false)
{

    private record Map(ImmutableDictionary<Vec2, char> Grid, Vec2 Size)
    {
        public void PrintAntiNodes(IEnumerable<Vec2> antiNodes)
        {
            Console.WriteLine();

            for (var r = 0; r < Size.R; r++)
            {
                for (var c = 0; c < Size.C; c++)
                {
                    if (Grid.ContainsKey(new Vec2(r, c)))
                    {
                        Console.Write(Grid[new Vec2(r, c)]);
                    }
                    else if (antiNodes.Contains(new Vec2(r, c)))
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }
    }
    
    protected override long Part1(string input)
    {
        var charGrid = ParseInput(input);

        var map = MakeGridDict(charGrid);

        var antiNodes = map.Grid.GroupBy(pair => pair.Value)
            .SelectMany(grouping => AntiNodes(grouping.Key, [..grouping.Select(pair => pair.Key)], map.Size))
            .ToFrozenSet();
        
        map.PrintAntiNodes(antiNodes);

        return antiNodes.Count();
    }

    private FrozenSet<Vec2> AntiNodes(char type, ImmutableArray<Vec2> antennaLocations, Vec2 size)
    {
        var pairs = from a1 in antennaLocations from a2 in antennaLocations where a1 != a2 select new { a1, a2 };

        var antinodes = pairs.SelectMany(p => AntiNodesForPair(p.a1, p.a2, size)).Distinct().ToArray();
        
        Debug.WriteLine($"For {type}; found antinodes: {string.Join(',', antinodes.OrderBy(n => n.R).ThenBy(n => n.C))}");
        
        return antinodes.ToFrozenSet();
    }

    private FrozenSet<Vec2> AntiNodesForPair(Vec2 a1, Vec2 a2, Vec2 size)
    {
        var diff = a2 - a1;

        Vec2[] options = [a1 - diff, a1 + diff, a2 - diff, a2 + diff];

        var antiNodes = options
            .Where(
                o =>
                    o.IsInGrid(size) &&
                    (Math.Abs(o.Distance(a1) - 2 * o.Distance(a2)) < Epsilon ||
                    Math.Abs(o.Distance(a2) - 2 * o.Distance(a1)) < Epsilon))
            .ToFrozenSet();
        
        Trace.Assert(antiNodes.Count <= 2);
        
        Trace.Assert(antiNodes.All(n => n.IsInGrid(size)));
        
        return antiNodes;
    }

    private const double Epsilon = 1e-6;

    private Map MakeGridDict(char[][] charGrid)
    {
        var size = new Vec2(charGrid.Length, charGrid[0].Length);

        var grid = new Dictionary<Vec2, char>();
        
        for (int r = 0; r < size.R; r++)
        {
            for (int c = 0; c < size.C; c++)
            {
                if (charGrid[r][c] == '.') continue;

                grid.Add(new Vec2(r, c), charGrid[r][c]);
            }
        }
        
        return new Map(grid.ToImmutableDictionary(), size);
    }

    protected override long Part2(string input)
    {
        var charGrid = ParseInput(input);

        var map = MakeGridDict(charGrid);
        
        foreach (var grouping in map.Grid.GroupBy(pair => pair.Value))
        {
            Debug.WriteLine($"{grouping.Key}: {string.Join(',', grouping.Select(p => p.Key))}");
        }

        var antiNodes = map.Grid.GroupBy(pair => pair.Value)
            .SelectMany(grouping => AntiNodes2(grouping.Key, [..grouping.Select(pair => pair.Key)], map))
            .ToFrozenSet();
        
        map.PrintAntiNodes(antiNodes);
        
        return antiNodes.Count;
    }

    private FrozenSet<Vec2> AntiNodes2(char type, ImmutableArray<Vec2> antennaLocations, Map map)
    {
        var pairs = from a1 in antennaLocations from a2 in antennaLocations where a1 != a2 select new { a1, a2 };

        var antiNodes = pairs.SelectMany(p => AntiNodesForPair2(p.a1, p.a2, map.Size)).Distinct().ToArray();
        
        return antiNodes.ToFrozenSet();
    }

    private FrozenSet<Vec2> AntiNodesForPair2(Vec2 a1, Vec2 a2, Vec2 size)
    {
        var maxDim = 2 * Math.Max(size.R, size.C);
        
        var diff = (a1 - a2).Normalized();
        
        // TODO: Making an equation for the line is probably nicer at this point
        var antiNodes = Enumerable
            .Range(-maxDim, maxDim)
            .Select(i => a1 + i * diff)
            .Where(o => o.IsInGrid(size))
            .Concat([a1, a2])
            .ToFrozenSet();
        
        return antiNodes;
    }
    
    private static char[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(l => l.ToCharArray()).ToArray();
    }

    [Test]
    [TestCase(1, 14)]
    [TestCase(2, 34)]
    public void Example(long part, long expected)
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

internal static class Vec2Extensions
{
    public static long ManhattanDistance(this Vec2 a, Vec2 b)
    {
        return Math.Abs(a.R - b.R) + Math.Abs(a.C - b.C); 
    }
    
    public static double Distance(this Vec2 a, Vec2 b)
    {
        return Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.C - b.C, 2)); 
    }
    
    public static bool IsInGrid(this Vec2 position, Vec2 gridSize)
    {
        return position is { C: >= 0, R: >= 0 } && position.C < gridSize.C && position.R < gridSize.R;
    }

    public static Vec2 Normalized(this Vec2 x)
    {
        // Turned out not to be needed...
        var gcd = (int) BigInteger.GreatestCommonDivisor(x.C, x.R);

        if (gcd == 1) return x;

        return new Vec2(x.R / gcd, x.C / gcd);
    }
}