using System.Collections.Frozen;
using System.Diagnostics;
using SkiaSharp;

namespace AoC2024;

public class Day18() : AoCDay(day: 18, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        return Part1(input, false);
    }

    static long Part1(string input, bool isTest)
    {
        var worldSize = isTest ? 7 : 71;
        var steps = isTest ? 12 : 1024;
        var gridSize = new Vec2(worldSize, worldSize);

        var positions = ParseInput(input);

        var blockedPositions = positions[..steps].ToFrozenSet();
        Trace.Assert(blockedPositions.Count == steps);
        
        return FindPath(blockedPositions, gridSize);
    }

    private static int FindPath(FrozenSet<Vec2> blockedPositions, Vec2 gridSize)
    {
        var costs = new Dictionary<Vec2, int>();
        var queue = new PriorityQueue<Vec2, int>();
        
        costs[(0, 0)] = 0;
        queue.Enqueue((0, 0), costs[(0, 0)]);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = costs[current];
            
            if (current == gridSize - (1, 1))
            {
                return currentCost;
            }

            foreach (var n in current.Neighbours(gridSize))
            {
                if (blockedPositions.Contains(n)) continue;
                
                if (!costs.TryGetValue(n, out var neighbourCost))
                {
                    costs[n] = currentCost + 1;
                    queue.Enqueue(n, currentCost + 1);
                }
                else
                {
                    // Found a lower cost path
                    if (currentCost + 1 < neighbourCost)
                    {
                        throw new InvalidOperationException("The square grid with equal costs should prevent finding a lower cost...");
                    }
                }
            }
        }
        
        return int.MaxValue;
    }

    protected override long Part2(string input)
    {
        return Part2(input, false);
    }

    private long Part2(string input, bool isTest)
    {
        var worldSize = isTest ? 7 : 71;
        var gridSize = new Vec2(worldSize, worldSize);
        var minSteps = isTest ? 12 : 1024;

        var positions = ParseInput(input);

        for (var steps = minSteps; steps <= positions.Count; steps++)
        {
            var blockedPositions = positions[..steps].ToFrozenSet();
            
            if (FindPath(blockedPositions, gridSize) == int.MaxValue)
            {
                Console.WriteLine($"X: {positions[steps - 1].C}; Y; {positions[steps - 1].R}");
                Console.WriteLine($"{positions[steps - 1].C},{positions[steps - 1].R}");
                
                return 0;
            }
        }

        return -1;
    }

    private static List<Vec2> ParseInput(string input)
    {
        return input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseCoord)
            .ToList();
    }

    private static Vec2 ParseCoord(string line)
    {
        var parsed = line.Split(',').Select(int.Parse).ToArray();

        if (parsed.Length != 2)
        {
            throw new ArgumentException("Line was not a 2d coordinate", nameof(line));
        }

        return new Vec2(parsed[1], parsed[0]);
    }
    
    [Test]
    [TestCase(1, 22)]
    [TestCase(2, 0)]
    public void Example(int part, long expected)
    {
        var result = part switch
        {
            1 => Part1(ExamplePart1, true),
            2 => Part2(ExamplePart2, true),
            _ => throw new ArgumentOutOfRangeException(nameof(part)),
        };

        Assert.That(result, Is.EqualTo(expected));
    }
}