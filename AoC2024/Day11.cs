using System.Collections.Immutable;

namespace AoC2024;

public class Day11() : AoCDay(day: 11, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var stones = ParseInput(input);

        return ComputeResult(stones, 25);
    }

    private long ComputeResult(List<long> stones, int maxSteps)
    {
        var counter = new Dictionary<long, long>();
        stones.ForEach(s => counter[s] = 1);
        
        for (var i = 0; i < maxSteps; i++)
        {
            // Console.WriteLine($"\nIteration {i}");
            //
            // foreach (var stone in counter.Keys)
            // {
            //     Console.WriteLine($"{stone}: {counter[stone]}");
            // }
            
            counter = Step(counter);
        }

        return counter.Values.Sum();
    }

    private static Dictionary<long, long> Step(Dictionary<long, long> counter)
    {
        var newCounter = new Dictionary<long, long>();
        
        foreach (var stone in counter.Keys)
        {
            var newStones = ApplyRules(stone);
            
            newStones.ForEach(newStone =>
            {
                if (newCounter.ContainsKey(newStone))
                {
                    newCounter[newStone] += counter[stone];
                }
                else
                {
                    newCounter[newStone] = counter[stone];
                }
            });
        }
        
        return newCounter;
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

        return ComputeResult(stones, 75);
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