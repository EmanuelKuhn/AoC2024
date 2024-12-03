using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC2024;

public partial class Day3() : AoCDay(day: 3, hasTwoInputs: false)
{
    private record Mul(int X, int Y)
    {
        public int Result => X * Y;
    }
    
    protected override long Part1(string input)
    {
        return MulRegex()
            .Matches(input)
            .Select(m => ParseMul(m.Groups))
            .Sum(mul => mul.Result);
    }
 
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)")]
    private static partial Regex MulRegex();
    
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)")]
    private static partial Regex InstructionRegex();

    private static Mul ParseMul(GroupCollection matchGroups)
    {
        Trace.Assert(matchGroups.Count == 3);
        
        return new Mul(int.Parse(matchGroups[1].Value), int.Parse(matchGroups[2].Value));
    }
    
    protected override long Part2(string input)
    {
        var isEnabled = true;
        var result = 0;
        
        foreach (Match match in InstructionRegex().Matches(input))
        {
            switch (match.Value)
            {
                case "do()":
                    isEnabled = true;
                    break;
                case "don't()":
                    isEnabled = false;
                    break;
                default:
                {
                    if (isEnabled)
                    {
                        result += ParseMul(match.Groups).Result;   
                    }
                    
                    break;
                }
            }
        }

        return result;
    }
    
    [Test]
    [TestCase(1, 161)]
    [TestCase(2, 48)]
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