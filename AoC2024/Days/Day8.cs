﻿using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace AoC2024;

public class Day8() : AoCDay(day: 8, hasTwoInputs: false)
{

    private record Map(ImmutableDictionary<Vec2, char> Grid, Vec2 Size)
    {
        public void PrintAntiNodes(ISet<Vec2> antiNodes)
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
                    (o.ManhattanDistance(a1) == 2 * o.ManhattanDistance(a2) ||
                     o.ManhattanDistance(a2) == 2 * o.ManhattanDistance(a1)))
            .ToFrozenSet();
        
        Trace.Assert(antiNodes.Count <= 2);
        
        Trace.Assert(antiNodes.All(n => n.IsInGrid(size)));
        
        return antiNodes;
    }
    
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

        var antiNodes = map.Grid.GroupBy(pair => pair.Value)
            .SelectMany(grouping => AntiNodes2([..grouping.Select(pair => pair.Key)], map))
            .ToFrozenSet();
        
        map.PrintAntiNodes(antiNodes);
        
        return antiNodes.Count;
    }

    private FrozenSet<Vec2> AntiNodes2(ImmutableArray<Vec2> antennaLocations, Map map)
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
            .Range(-(int)maxDim, (int)maxDim)
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