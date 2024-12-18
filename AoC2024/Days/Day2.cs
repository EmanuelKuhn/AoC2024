using System.Collections.Immutable;

namespace AoC2024;

public class Day2() : AoCDay(day: 2, hasTwoInputs: false)
{
    protected override long Part1(string input)
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

    protected override long Part2(string input)
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
    [TestCase(1, 2)]
    [TestCase(2, 4)]
    public void Example(int part, long expected)
    {
        var solveMethod = GetSolveExamplePart(part);
        
        var result = solveMethod();
        
        Assert.That(result, Is.EqualTo(expected));
    }
}