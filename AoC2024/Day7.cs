using System.Collections.Frozen;

namespace AoC2024;

public class Day7() : AoCDay(day: 7, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var entries = ParseInput(input);

        return entries.Where(e => IsValid1(e.testValue, e.numbers)).Select(e => e.testValue).Sum();
    }

    protected override long Part2(string input)
    {
        var entries = ParseInput(input);

        return entries.Where(e => IsValid2(e.testValue, e.numbers)).Select(e => e.testValue).Sum();
    }
    
    private bool IsValid1(long testValue, long[] numbers) => IsValid(testValue, numbers, false);
    
    private bool IsValid2(long testValue, long[] numbers) => IsValid(testValue, numbers, true);
    
    private bool IsValid(long testValue, long[] numbers, bool allowConcat)
    {
        if (numbers.Length == 1)
        {
            return testValue == numbers[0];
        }

        List<long> options =
        [
            numbers[0] * numbers[1], 
            numbers[0] + numbers[1],
        ];

        if (allowConcat)
        {
            options.Add(long.Parse($"{numbers[0].ToString()}{numbers[1].ToString()}"));
        }
        
        return options.Any(o => IsValid(testValue, [o, ..numbers.Skip(2)], allowConcat));
    }

    private static List<(long testValue, long[] numbers)> ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(ParseLine).ToList();
    }

    private static (long testValue, long[] numbers) ParseLine(string line)
    {
        var testValue = long.Parse(line.Split(": ").First());
        
        var numbers = line.Split(": ").Skip(1).First().Split(" ").Select(long.Parse).ToArray();
        
        return (testValue, numbers);
    }


    [Test]
    [TestCase(1, 3749)]
    [TestCase(2, 11387)]
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