using System.Diagnostics;

namespace AoC2024;

public sealed class FancyStopWatch(string? label) : IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public void Dispose()
    {
        _stopwatch.Stop();

        Console.WriteLine(label is not null
            ? $"[{label}] Elapsed time: {_stopwatch.Elapsed}"
            : $"Elapsed time: {_stopwatch.Elapsed}");
    }
}