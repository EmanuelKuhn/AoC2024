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

        var blockedPositions = positions[..steps].ToHashSet();
        Trace.Assert(blockedPositions.Count == steps);

        var costs = new Dictionary<Vec2, long>();
        var queue = new PriorityQueue<Vec2, long>();
        
        costs[(0, 0)] = 0;
        queue.Enqueue((0, 0), costs[(0, 0)]);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = costs[current];
            
            // Console.WriteLine($"{current}: {currentCost}");
            
            if (current == (worldSize - 1, worldSize - 1)) return currentCost;
            
            foreach (var n in current.Neighbours(gridSize))
            {
                if (blockedPositions.Contains(n)) continue;
                
                if (!costs.TryGetValue(n, out var cost))
                {
                    costs[n] = currentCost + 1;
                    queue.Enqueue(n, currentCost + 1);
                }
                else
                {
                    // Found a lower cost path
                    if (currentCost + 1 < cost)
                    {
                        // costs[n] = currentCost + 1;
                        throw new NotImplementedException("Maybe in this grid a lower cost is never found?");
                    }
                }
            }
        }
        
        throw new InvalidOperationException("Should have found the end, but didn't...");
    }
    
    protected override long Part2(string input)
    {
        return 0;
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
            2 => Part2(ExamplePart2),
            _ => throw new ArgumentOutOfRangeException(nameof(part)),
        };

        Assert.That(result, Is.EqualTo(expected));
    }
}