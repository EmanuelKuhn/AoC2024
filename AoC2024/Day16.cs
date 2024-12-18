namespace AoC2024;

public class Day16() : AoCDay(day: 16, hasTwoInputs: false)
{
    private record struct State(Vec2 Position, char Direction)
    {
        public IEnumerable<(State, long)> Neighbours(Vec2 gridSize)
        {
            var neighbours = new List<(State, long)>();

            neighbours.Add((this with { Direction = Direction.RotatedRight()}, 1000));
            neighbours.Add((this with { Direction = Direction.RotatedLeft()}, 1000));

            var forwardPosition = Position.Moved(Direction.AsUnit());

            if (forwardPosition.IsInGrid(gridSize))
            {
                neighbours.Add((this with { Position = forwardPosition}, 1));
            }
            
            return neighbours;
        }
    }

    private class World
    {
        public char[][] Map { get; }
        
        private Vec2 WorldSize { get; init; }

        private Vec2 Start { get; }
        private Vec2 End { get; }
        
        public World(char[][] map)
        {
            WorldSize = new Vec2(map.Length, map[0].Length);

            Map = map;

            Start = map.FindFirst('S')!.Value;
            End = map.FindFirst('E')!.Value;
        }
        
        public long FindPath()
        {
            var costs = new Dictionary<State, long>();
            var queue = new PriorityQueue<State, long>();
        
            costs[new State(Start, '>')] = 0;
            queue.Enqueue(new State(Start, '>'), costs[new State(Start, '>')]);

            List<long> endCosts = [];
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentCost = costs[current];
            
                if (current.Position == End)
                {
                    endCosts.Add(currentCost);
                }

                foreach (var (neighbour, costToNeighbor) in current.Neighbours(WorldSize))
                {
                    if (Map.At(neighbour.Position) == '#') continue;
                    
                    var newCost = currentCost + costToNeighbor;
                    
                    if (!costs.TryGetValue(neighbour, out var currentCostToNeighbor))
                    {
                        costs[neighbour] = newCost;
                        queue.Enqueue(neighbour, newCost);
                    }
                    else
                    {
                        if (newCost >= currentCostToNeighbor) continue;

                        // Found a lower cost path:
                        costs[neighbour] = newCost;
                            
                        // Remove previous entry and add back with new cost
                        queue = new PriorityQueue<State, long>(
                            queue.UnorderedItems
                                .Where(item => item.Element != neighbour)
                                .Append((neighbour, newCost)));
                    }
                }
            }

            return endCosts.Count > 0 ? endCosts.Min() : -1;
        }
    }
    
    protected override long Part1(string input)
    {
        var map = ParseInput(input);

        var world = new World(map);

        return world.FindPath();
    }
    
    protected override long Part2(string input)
    {
        return 0;
    }

    private static char[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        return lines.Select(l => l.ToCharArray()).ToArray();
    }
    
    [Test]
    [TestCase(1, 11048)]
    [TestCase(2, 0)]
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

file static class GridExtensions
{
    public static Vec2? FindFirst<T>(this T[][] grid, T value)
    where T : struct, IEquatable<T>
    {
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                if (grid[r][c].Equals(value))
                {
                    return new Vec2(r, c);
                }
            }
        }

        return null;
    }
    
    public static T At<T>(this T[][] grid, Vec2 position)
    {
        return grid[position.R][position.C];
    }

    public static bool IsInGrid<T>(this T[][] grid, Vec2 position)
    {
        return position is { C: >= 0, R: >= 0 } && position.R < grid.Length && position.C < grid[position.R].Length;
    }
}