using System.Diagnostics;

namespace AoC2024;

public class Day1
{
    private static long Part1(string input)
    {
        var (lefts, rights) = ParseInput(input);
        
        lefts.Sort();
        rights.Sort();
        
        var distances = lefts.Zip(rights, (a, b) => Math.Abs(a - b));

        return distances.Sum();
    }

    private static long Part2(string input)
    {
        var (lefts, rights) = ParseInput(input);

        var countsRight = rights.CountBy(r => r).ToDictionary(pair => pair.Key, pair => (long) pair.Value);

        return lefts
            .Select(left => countsRight.TryGetValue(left, out var right) ? left * right : 0)
            .Sum();
    }
    
    private static (List<long> lefts, List<long> rights) ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        List<long> lefts = [];
        List<long> rights = [];
        
        foreach (var valueTuple in lines.Select(ParseLine))
        {
            lefts.Add(valueTuple.Item1);
            rights.Add(valueTuple.Item2);
        }

        return (lefts, rights);
    }
    
    private static (long left, long right) ParseLine(string line)
    {
        var parts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Trace.Assert(parts.Length == 2, $"Expected parts.Length == 2, but parts.Length: {parts.Length}");
        
        return (long.Parse(parts[0]), long.Parse(parts[1]));
    }
    
    [Test]
    public void Example1()
    {
        var input = File.ReadAllText("Examples/1.txt");
        Console.WriteLine(input);

        var result = Part1(input);
        
        Assert.That(result, Is.EqualTo(11));
    }

    [Test]
    public void Example2()
    {
        var input = File.ReadAllText("Examples/1.txt");
        Console.WriteLine(input);
    
        var result = Part2(input);
        
        Assert.That(result, Is.EqualTo(31));
    }
    
    [Test]
    public void Solve()
    {
        var input = File.ReadAllText("Inputs/1.txt");
        var part1 = Part1(input);
        
        Console.WriteLine("Day 1, part 1:");
        Console.WriteLine(part1);
        
        var part2 = Part2(input);
        
        Console.WriteLine("\nDay 1, part 2:");
        Console.WriteLine(part2);
    }
}