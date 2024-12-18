using System.Collections.Immutable;

namespace AoC2024;

public class Day10() : AoCDay(day: 10, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var grid = ParseInput(input);

        var trailheads = grid.Positions().Where(t => grid[t.R][t.C] == 0).ToList();

        var scorer = new TrailheadScorer(grid);
        
        var scores = trailheads.Select(scorer.CountReachable).ToList();
        
        return scores.Sum();
    }

    private class TrailheadScorer(int[][] grid)
    {
        private readonly Dictionary<Vec2, ImmutableHashSet<Vec2>> _memo = new();

        public int CountReachable(Vec2 point)
        {
            return FindReachable(point).Count;
        }
        
        private ImmutableHashSet<Vec2> FindReachable(Vec2 start)
        {
            if (_memo.TryGetValue(start, out var value))
            {
                return value;
            }

            if (grid[start.R][start.C] == 9)
            {
                return [start];
            }
            
            var neighbours = grid
                .Neighbours(start)
                .Where(p => grid[p.R][p.C] == grid[start.R][start.C] + 1)
                .ToArray();

            var result = neighbours.SelectMany(FindReachable).ToImmutableHashSet();
            _memo[start] = result;
            
            return result;
        }
    }
    
    private class TrailheadRater(int[][] grid)
    {
        private readonly Queue<ImmutableArray<Vec2>> _queue = new();
        private readonly Dictionary<Vec2, HashSet<ImmutableArray<Vec2>>> _reachableTrails = [];
        
        public void Analyze()
        {
            // Go from peaks to trailheads
            var targets = grid.Positions()
                .Where(t => grid[t.R][t.C] == 9)
                .ToList();

            targets.ForEach(target => FindTrails([target]));
        }

        private void FindTrails(ImmutableArray<Vec2> pathEnding)
        {
            _queue.Enqueue(pathEnding);

            while (_queue.Count > 0)
            {
                var currentPath = _queue.Dequeue();
                var current = currentPath[0];
                
                if (grid[current.R][current.C] == 0)
                {
                    if (_reachableTrails.TryGetValue(current, out var reachable))
                    {
                        reachable.Add(currentPath);
                    }
                    else
                    {
                        _reachableTrails[current] = [currentPath];
                    }
                    
                    continue;
                }
                
                var neighbours = grid
                    .Neighbours(current)
                    .Where(p => grid[p.R][p.C] == grid[current.R][current.C] - 1)
                    .ToList();
                
                // Paths are always unique (the ending is different), so no need to check if the path was already visited/enqued 
                neighbours.ForEach(neighbour => _queue.Enqueue([neighbour, ..currentPath]));
            }
        }

        public int Rate(Vec2 start)
        {
            if (_reachableTrails.TryGetValue(start, out var value))
            {
                return value.Count;
            }
            
            throw new InvalidOperationException($"Expected {nameof(_reachableTrails)} to be filled in advance (Count is: {_reachableTrails.Count})");
        }
    }

    protected override long Part2(string input)
    {
        var grid = ParseInput(input);

        var trailheads = grid.Positions().Where(t => grid[t.R][t.C] == 0).ToList();

        var scorer = new TrailheadRater(grid);
        scorer.Analyze();
        
        var scores = trailheads.Select(scorer.Rate).ToList();
        
        return scores.Sum();
    }

    private static int[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(l => l.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
    }
    

    [Test]
    [TestCase(1, 36)]
    [TestCase(2, 81)]
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

file static class GridExtensions {
    public static IEnumerable<Vec2> Positions<T>(this T[][] grid) {
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                yield return new Vec2(r, c);
            }
        }
    }
    
    public static bool IsInGrid<T>(this T[][] grid, Vec2 position)
    {
        return position is { C: >= 0, R: >= 0 } && position.R < grid.Length && position.C < grid[position.R].Length;
    }

    public static IEnumerable<Vec2> Neighbours<T>(this T[][] grid, Vec2 position)
    {
        (int, int)[] offsets = [(0, 1), (0, -1), (1, 0), (-1, 0)];
        var coords = offsets.Select(o => new Vec2(position.R + o.Item1, position.C + o.Item2)).ToArray();

        return coords.Where(grid.IsInGrid);
    }
}
