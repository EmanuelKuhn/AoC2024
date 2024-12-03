using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AoC2024;

public partial class Day3() : AoCDay(day: 3, hasTwoInputs: false)
{
    private interface IInstruction
    {
    }
    
    public record Mul(int x, int y) : IInstruction;
    public record Do() : IInstruction;
    public record DoNot() : IInstruction;
    
    protected override long Part1(string input)
    {
        List<Mul> instructions = [];
        
        foreach (Match match in MulRegex().Matches(input))
        {
            var mul = ParseMul(match.Value);

            if (mul is not null)
            {
                instructions.Add(mul);
            }
        }

        return instructions.Select(mul => mul.x * mul.y).Sum();
    }

    private Mul? ParseMul(string input)
    {
        var digitsStrings = input.Split('(').Skip(1).First().Split(')').First().Split(',');
        
        if (digitsStrings.Any(x => x.Length > 3)) return null;
        
        var digits = digitsStrings.Select(int.Parse).ToImmutableArray();

        return digits.Length != 2 ? null : new Mul(digits.First(), digits.Skip(1).First());
    }
    
    [GeneratedRegex(@"mul\(\d+,\d+\)")]
    private static partial Regex MulRegex();

    [GeneratedRegex(@"do\(\)")]
    private static partial Regex DoRegex();

    [GeneratedRegex(@"don't\(\)")]
    private static partial Regex DonotRegex();
    
    [GeneratedRegex(@"mul\(\d+,\d+\)|do\(\)|don't\(\)")]
    private static partial Regex InstructionRegex();

    
    protected override long Part2(string input)
    {
        List<IInstruction> instructions = [];
        
        foreach (Match match in InstructionRegex().Matches(input))
        {
            if (DoRegex().IsMatch(match.Value))
            {
                instructions.Add(new Do());
            } else if (DonotRegex().IsMatch(match.Value))
            {
                instructions.Add(new DoNot());
            }
            else
            {
                var mul = ParseMul(match.Value);

                if (mul is not null)
                {
                    instructions.Add(mul);
                }
            }
        }

        var isEnabled = true;
        var result = 0;

        foreach (var instruction in instructions)
        {
            switch (instruction)
            {
                case Do:
                    isEnabled = true;
                    break;
                case DoNot:
                    isEnabled = false;
                    break;
                case Mul mul:
                    if (isEnabled)
                    {
                        result += mul.x * mul.y;
                    }

                    break;
            }
        }

        return result;
    }
    
    [Test]
    [TestCase(1, 161)]
    [TestCase(2, 48)]
    public void Example(int part, long expected)
    {
        var solveMethod = GetSolveExamplePart(part);
        
        var result = solveMethod();
        
        Assert.That(result, Is.EqualTo(expected));
    }
}