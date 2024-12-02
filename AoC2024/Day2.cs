using System.Collections.Immutable;

namespace AoC2024;

public class Day2
{
    private const int Day = 2;
    
    private static long Part1(string input)
    {
        var reports = ParseInput(input);
        
        return reports.Select(IsSafe).Count(x => x);
    }

    private static bool IsSafe(ImmutableArray<long> report)
    {
        var diffs = report.Zip(report.Skip(1), (a, b) => b - a).ToArray();
        
        return diffs[0] switch
        {
            0 => false,
            < 0 => diffs.All(d => d is < 0 and >= -3),
            > 0 => diffs.All(d => d is > 0 and <= 3)
        };
    }

    private static long Part2(string input)
    {
        var reports = ParseInput(input);
        
        return reports.Select(IsSafeWithProblemDampener).Count(x => x);
    }
    
    private static bool IsSafeWithProblemDampener(ImmutableArray<long> report)
    {
        return report.Select((t, i) => report.RemoveAt(i)).Any(IsSafe);
    }
    
    private static ImmutableArray<ImmutableArray<long>> ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        return [..lines.Select(ParseLine)];
    }
    
    private static ImmutableArray<long> ParseLine(string line)
    {
        var parts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        return [..parts.Select(long.Parse)];
    }
    
    [Test]
    public void Example1()
    {
        var input = File.ReadAllText($"Examples/{Day}.txt");
        Console.WriteLine(input);

        var result = Part1(input);
        
        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void Example2()
    {
        var input = File.ReadAllText($"Examples/{Day}.txt");
        Console.WriteLine(input);
    
        var result = Part2(input);
        
        Assert.That(result, Is.EqualTo(4));
    }
    
    [Test]
    public void Solve()
    {
        var input = File.ReadAllText($"Inputs/{Day}.txt");
        var part1 = Part1(input);
        
        Console.WriteLine($"Day {Day}, part 1:");
        Console.WriteLine(part1);
        
        var part2 = Part2(input);
        
        Console.WriteLine($"\nDay {Day}, part 2:");
        Console.WriteLine(part2);
    }
}