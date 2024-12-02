using System.Collections.Immutable;
using System.Diagnostics;

namespace AoC2024;

public static class Benchmarker
{
    public record BenchmarkResult(ImmutableArray<TimeSpan> ElapsedTimes);
    
    public static BenchmarkResult Benchmark(this Action method, long iterations)
    {
        List<TimeSpan> times = [];
        
        for (var i = 0; i < iterations; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            method();
            stopwatch.Stop();
            
            times.Add(stopwatch.Elapsed);
        }
        
        return new BenchmarkResult([..times]);
    }

    public static void Summarize(this BenchmarkResult result, string label)
    {
        var average = result.ElapsedTimes.Average(ts => ts.Microseconds);
        
        Console.WriteLine($"[{label}] Average: {average} microseconds of {result.ElapsedTimes.Length} runs");
    }
}