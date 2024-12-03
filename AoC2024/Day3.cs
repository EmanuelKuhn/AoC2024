using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AoC2024;

public partial class Day3() : AoCDay(day: 3, hasTwoInputs: false)
{
    private interface IInstruction
    {
    }
    
    public record Mul(int X, int Y) : IInstruction;
    public record Do() : IInstruction;
    public record DoNot() : IInstruction;
    
    protected override long Part1(string input)
    {
        List<Mul> instructions = [];
        
        foreach (Match match in MulRegex().Matches(input))
        {
            instructions.Add(ParseMul(match.Groups));
        }

        return instructions.Select(mul => mul.X * mul.Y).Sum();
    }
 
    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex MulRegex();
    
    [GeneratedRegex(@"mul\((\d+),(\d+)\)|do\(\)|don't\(\)")]
    private static partial Regex InstructionRegex();

    private static Mul ParseMul(GroupCollection matchGroups)
    {
        return new Mul(int.Parse(matchGroups[1].Value), int.Parse(matchGroups[2].Value));
    }
    
    protected override long Part2(string input)
    {
        List<IInstruction> instructions = [];
        
        foreach (Match match in InstructionRegex().Matches(input))
        {
            switch (match.Value)
            {
                case "do()":
                    instructions.Add(new Do());
                    break;
                case "don't()":
                    instructions.Add(new DoNot());
                    break;
                default:
                {
                    Trace.Assert(match.Groups.Count == 3);
                    
                    instructions.Add(ParseMul(match.Groups));
                    break;
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
                        result += mul.X * mul.Y;
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