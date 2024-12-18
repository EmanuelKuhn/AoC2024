using System.Collections.Immutable;

namespace AoC2024;

public class Day11() : AoCDay(day: 11, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var stones = ParseInput(input);

        return StonesCount(stones, 25);
    }

    private long StonesCount(List<long> stones, int iterations)
    {
        return stones.Count + stones.Select(stone => ComputeSplits(stone, iterations)).Sum();
    }

    private readonly Dictionary<(long, long), long> _memo = new();
    
    private long ComputeSplits(long stone, int iterations)
    {
        if (iterations == 0)
        {
            return 0;
        }
        
        if (_memo.ContainsKey((stone, iterations)))
        {
            return _memo[(stone, iterations)];
        }
        
        long splits = 0;
        
        var next = ApplyRules(stone);

        if (next.Count == 1)
        {
            splits += ComputeSplits(next.First(), iterations - 1);
        }
        else
        {
            splits += 1;
            splits += ComputeSplits(next.First(), iterations - 1) + ComputeSplits(next.Last(), iterations - 1);
        }
        
        _memo.Add((stone, iterations), splits);
        
        return splits;
    }

    private static List<long> ApplyRules(long stone)
    {
        if (stone == 0)
        {
            return [1];
        }

        var stoneString = stone.ToString();
        
        if (stoneString.Length % 2 == 0)
        {
            return [
                long.Parse(stoneString[..(stoneString.Length / 2)]),
                long.Parse(stoneString[(stoneString.Length / 2)..]),
            ];
        }

        return [stone * 2024];
    }

    protected override long Part2(string input)
    {
        var stones = ParseInput(input);

        return StonesCount(stones, 75);
    }

    private static List<long> ParseInput(string input)
    {
        var line = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).First();
        
        var stones = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return [..stones.Select(long.Parse)];
    }
    

    [Test]
    [TestCase(1, 55312)]
    [TestCase(2, 65601038650482)]
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