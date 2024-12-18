using System.Collections.Frozen;
using System.Diagnostics;
using System.Numerics;

namespace AoC2024;

public class Day13() : AoCDay(day: 13, hasTwoInputs: false)
{
    record Config(Vec2 ButtonA, Vec2 ButtonB, Vec2 Prize);
    
    protected override long Part1(string input)
    {
        var configs = ParseInput(input).ToList();
        
        return configs.Select(SolveConfig).Where(v => v.HasValue).Select(v => v!.Value).Sum();
    }

    private static long? SolveConfig(Config cfg)
    {
        var nominator = cfg.Prize.R* cfg.ButtonA.C - cfg.Prize.C * cfg.ButtonA.R;
        var denominator = cfg.ButtonA.C * cfg.ButtonB.R - cfg.ButtonB.C * cfg.ButtonA.R;

        if (denominator == 0) throw new InvalidOperationException("Multiple solutions would be possible and are not supported.");
        
        if (nominator % denominator != 0) return null;

        var b = (long) nominator / denominator;

        var nominatorA = cfg.Prize.C* cfg.ButtonB.R - cfg.Prize.R * cfg.ButtonB.C;
        
        if (nominatorA % denominator != 0) return null;
        
        var a = (long) nominatorA / denominator;
        
        return a * 3 + b;
    }
    
    protected override long Part2(string input)
    {
        var measurementError = 10000000000000 * new Vec2(1, 1);
        
        var configs = ParseInput(input).Select(cfg => cfg with {Prize = cfg.Prize + measurementError}).ToList();
        
        return configs.Select(SolveConfig).Where(v => v.HasValue).Select(v => v!.Value).Sum();
    }

    private static IEnumerable<Config> ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        
        while (lines.Count > 2)
        {
            Trace.Assert(lines[0].StartsWith("Button A:"));
            var a = ParseButton(lines[0]);
            var b = ParseButton(lines[1]);
            var prize = ParsePrize(lines[2]);
            
            lines.RemoveRange(0, 3);
            
            yield return new Config(a, b, prize);
        }
    }

    private static Vec2 ParseButton(string line)
    {
        var x = int.Parse(line.Split("X+").Last().Split(",").First());
        var y = int.Parse(line.Split("Y+").Last());
        
        return new Vec2(x, y);
    }

    private static Vec2 ParsePrize(string line)
    {
        var x = int.Parse(line.Split("X=").Last().Split(",").First());
        var y = int.Parse(line.Split("Y=").Last());
        
        return new Vec2(x, y);
    }

    [Test]
    [TestCase(1, 480)]
    [TestCase(2, 875318608908)]
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