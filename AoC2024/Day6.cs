using System.Collections.Frozen;

namespace AoC2024;

public class Day6() : AoCDay(day: 6, hasTwoInputs: false)
{
    record struct Guard(Vec2 Position, char Direction);
    
    class World
    {
        public Guard Guard { get; private set; }

        private FrozenSet<Vec2> Obstructions { get; }
        
        public HashSet<Guard> Visited { get; }
        
        Vec2 Size { get; }

        private World(Guard guard, FrozenSet<Vec2> obstructions, HashSet<Guard> visited, Vec2 size)
        {
            Guard = guard;
            Obstructions = obstructions;
            Visited = visited;
            Size = size;
        }

        public World(Guard guard, FrozenSet<Vec2> obstructions, Vec2 size) : this(guard, obstructions, [guard], size) { }

        private (bool isValid, bool looped) Step()
        {
            if (NextGuard() is not { } nextGuard) return (false, false);

            var looped = Visited.Contains(nextGuard);
            
            Visited.Add(nextGuard);
            Guard = nextGuard;

            return (true, looped);
        }
        
        public bool WillLoop()
        {
            while (Step() is { isValid: true } result)
            {
                if (result.looped) return true;
            }

            return false;
        }
        
        public World Cloned() => new(Guard, Obstructions, Visited.AsEnumerable().ToHashSet(), Size);
        
        public World? Stepped()
        {
            var cloned = Cloned();
        
            return cloned.Step() is {isValid: true} ? cloned : null;
        }

        private Guard? NextGuard()
        {
            var direction = Guard.Direction;
            var maybeNewPosition = Guard.Position.Moved(direction);
            
            while (Obstructions.Contains(maybeNewPosition))
            {
                direction = direction.RotatedRight();
                maybeNewPosition = Guard.Position.Moved(direction);
            }

            if (!IsInGrid(maybeNewPosition))
            {
                return null;
            }
            
            return new Guard(maybeNewPosition, direction);
        }
        
        private bool IsInGrid(Vec2 position)
        {
            return position.C >= 0 && position.R >= 0 && position.C < Size.C && position.R < Size.R;
        }
    }
    
    protected override long Part1(string input)
    {
        var charGrid = ParseInput(input);
        var obstructions = LocateObstructions(charGrid).ToFrozenSet();

        var guard = LocateGuard(charGrid);

        var world = new World(guard, obstructions, new Vec2(charGrid.Length, charGrid[0].Length));
        
        while (world.Stepped() is {} nextWorld)
        {
            world = nextWorld;
        }
        
        return world.Visited.Select(g => g.Position).Distinct().Count();
    }

    protected override long Part2(string input)
    {
        var charGrid = ParseInput(input);
        var obstructions = LocateObstructions(charGrid).ToFrozenSet();

        var guard = LocateGuard(charGrid);

        var worldSize = new Vec2(charGrid.Length, charGrid[0].Length);
        
        var world = new World(guard, obstructions, worldSize);
        
        List<World> worlds = [];
        
        while (world.Stepped() is {} nextWorld)
        {
            world = nextWorld;
            worlds.Add(nextWorld);
        }

        var potentialObstructions = worlds.Select(w => w.Guard.Position).Distinct().ToFrozenSet();

        var possibleObstructions = new HashSet<Vec2>();
        
        foreach (var potentialObstruction in potentialObstructions)
        {
            world = new World(guard, [..obstructions, potentialObstruction], worldSize);

            if (world.WillLoop())
            {
                possibleObstructions.Add(potentialObstruction);
            }
        }
        
        Console.WriteLine($"Number of potential obstructions: {potentialObstructions.Count}");
        Console.WriteLine($"Number of possible obstructions: {possibleObstructions.Count}");

        return possibleObstructions.Count;
    }

    private static Vec2[] LocateObstructions(char[][] grid)
    {
        List<Vec2> obstructions = [];

        for (var r = 0; r < grid.Length; r++)
        {
            var row = grid[r];

            for (var c = 0; c < row.Length; c++)
            {
                if (row[c] == '#')
                {
                    obstructions.Add(new Vec2(r, c));
                }
            }
        }

        return obstructions.ToArray();
    }

    private Guard LocateGuard(char[][] grid)
    {
        for (int r = 0; r < grid.Length; r++)
        {
            var row = grid[r];

            for (int c = 0; c < row.Length; c++)
            {
                if (Directions.All.Contains(row[c]))
                {
                    return new Guard(new Vec2(r, c), row[c]);
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static char[][] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.Select(l => l.ToCharArray()).ToArray();
    }
    

    [Test]
    [TestCase(1, 41)]
    [TestCase(2, 6)]
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

public static class Directions
{
    public const char Up = '^';
    public const char Down = 'v';
    public const char Right = '>';
    public const char Left = '<';
        
    public static readonly char[] All = [Up, Right, Down, Left, Up];
        
    public static Vec2 AsUnit(this char direction)
    {
        return direction switch
        { 
            Up => new Vec2(-1, 0),
            Down => new Vec2(1, 0),
            Left => new Vec2(0, -1),
            Right => new Vec2(0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    
    public static char RotatedRight(this char direction)
    {
        return direction switch
        {
            Up => Right,
            Right => Down,
            Down => Left,
            Left => Up,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}