namespace AoC2024;

public abstract class AoCDay
{
    public required int Day;
    protected string InputPart1 { get; }
    protected string ExamplePart1 { get; }

    protected string InputPart2 { get; }
    protected string ExamplePart2 { get; }
    
    protected AoCDay(int day, bool hasTwoInputs)
    {
        Day = day;

        if (hasTwoInputs)
        {
            throw new NotImplementedException();
        }
        else
        {
            InputPart1 = File.ReadAllText($"Inputs/{Day}.txt");
            ExamplePart1 = File.ReadAllText($"Examples/{Day}.txt");
        
            InputPart2 = InputPart1;
            ExamplePart2 = ExamplePart1;   
        }
    }

    protected abstract long Part1(string input);
    
    protected abstract long Part2(string input);
    
    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void Benchmark(int part)
    {
        var solveMethod = GetSolvePart(part);
        
        Action f = () => solveMethod();

        f.Benchmark(10).Summarize($"Part {part}");
    }
    
    protected Func<long> GetSolvePart(int part)
    {
        return part switch
        {
            1 => () => Part1(InputPart1),
            2 => () => Part2(InputPart2),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Part should be 1 or 2.")
        };
    }
    
    protected Func<long> GetSolveExamplePart(int part)
    {
        return part switch
        {
            1 => () => Part1(ExamplePart1),
            2 => () => Part2(ExamplePart2),
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, "Part should be 1 or 2.")
        };
    }
    
    [TestCase(1)]
    [TestCase(2)]
    public void Solve(int part)
    {
        var solveMethod = GetSolvePart(part);
        
        long output;
        using (new FancyStopWatch($"Part {part}")) {
            output = solveMethod();
        }
        
        Console.WriteLine($"Day {Day}, part {part}:");
        Console.WriteLine(output);
        Console.WriteLine();
    }
}