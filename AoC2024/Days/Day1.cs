using System.Collections.Immutable;
using System.Diagnostics;

namespace AoC2024;

public class Day1() : AoCDay(day: 1, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var (lefts, rights) = ParseInput(input);
        
        lefts = lefts.Sort();
        rights = rights.Sort();
        
        var distances = lefts.Zip(rights, (a, b) => Math.Abs(a - b));

        return distances.Sum();
    }

    protected override long Part2(string input)
    {
        var (lefts, rights) = ParseInput(input);

        var countsRight = rights.CountBy(r => r).ToDictionary(pair => pair.Key, pair => (long) pair.Value);

        return lefts
            .Select(left => countsRight.TryGetValue(left, out var right) ? left * right : 0)
            .Sum();
    }
    
    private static (ImmutableArray<long> lefts, ImmutableArray<long> rights) ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        List<long> lefts = [];
        List<long> rights = [];
        
        foreach (var valueTuple in lines.Select(ParseLine))
        {
            lefts.Add(valueTuple.Left);
            rights.Add(valueTuple.Right);
        }

        return ([..lefts], [..rights]);
    }
    
    private record Pair(long Left, long Right);
    
    private static Pair ParseLine(string line)
    {
        var parts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Trace.Assert(parts.Length == 2, $"Expected parts.Length == 2, but parts.Length: {parts.Length}");
        
        return new Pair(long.Parse(parts[0]), long.Parse(parts[1]));
    }
    
    [Test]
    [TestCase(1, 11)]
    [TestCase(2, 31)]
    public void Example(int part, long expected)
    {
        var solveMethod = GetSolveExamplePart(part);
        
        var result = solveMethod();
        
        Assert.That(result, Is.EqualTo(expected));
    }
}