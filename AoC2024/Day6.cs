using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;

namespace AoC2024;

public class Day6() : AoCDay(day: 6, hasTwoInputs: false)
{
    [Flags]
    private enum Orientation
    {
        Up, 
        Down, 
        Left, 
        Right
    }
    
    private Orientation ParseDirection(char c)
    {
        return c switch
        {
            '^' => Orientation.Up,
            'v' => Orientation.Down,
            '<' => Orientation.Left,
            '>' => Orientation.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
    
    private static Orientation TurnRight(Orientation direction)
    {
        return direction switch
        {
            Orientation.Up => Orientation.Right,
            Orientation.Down => Orientation.Left,
            Orientation.Left => Orientation.Up,
            Orientation.Right => Orientation.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static ImmutableArray<char> _guardSymbols = ['^', 'v', '<', '>'];

    private record Vec2(int R, int C)
    {
        public Vec2 Move(Vec2 v)
        {
            return new Vec2(R + v.R, C + v.C);
        }
        
        
        
        public Vec2 MoveUnit(Orientation direction)
        {
            return Move(AsUnit(direction));
        }
        
        public static Vec2 AsUnit(Orientation direction)
        {
            return direction switch
            { 
                Orientation.Up => new Vec2(-1, 0),
                Orientation.Down => new Vec2(1, 0),
                Orientation.Left => new Vec2(0, -1),
                Orientation.Right => new Vec2(0, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
    
    private interface IGridItem
    {
        Vec2 Position { get; }
    }

    private class Guard(Vec2 position, Orientation direction) : IGridItem
    {
        public Vec2 Position { get; private set; } = position;
        public Orientation Direction { get; private set; } = direction;

        public void Move(FrozenSet<Vec2> obstructions)
        {
            var maybeNewPosition = Position.MoveUnit(Direction);

            while (obstructions.Contains(maybeNewPosition))
            {
                Direction = TurnRight(Direction);
                maybeNewPosition = Position.MoveUnit(Direction);
            }

            Position = maybeNewPosition;
        }
    }

    private record Obstruction(Vec2 Position) : IGridItem;

    protected override long Part1(string input)
    {
        var charGrid = ParseInput(input);

        // var gridDict = new Dictionary<(int x, int y), IGridItem>();
        var obstructions = LocateObstructions(charGrid).Select(o => o.Position).ToFrozenSet();

        var guard = LocateGuard(charGrid);
        
        var isInGrid = true;

        HashSet<Vec2> visited = [guard.Position];
        
        while (isInGrid)
        {
            guard.Move(obstructions);
            
            isInGrid = guard.Position.C < charGrid[0].Length && guard.Position.R < charGrid.Length;

            if (isInGrid)
            {
                visited.Add(guard.Position);
            }
        }

        return visited.Count;
    }

    private static Obstruction[] LocateObstructions(char[][] grid)
    {
        List<Obstruction> obstructions = [];

        for (var r = 0; r < grid.Length; r++)
        {
            var row = grid[r];

            for (var c = 0; c < row.Length; c++)
            {
                if (row[c] == '#')
                {
                    obstructions.Add(new Obstruction(new Vec2(r, c)));
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
                if (_guardSymbols.Contains(row[c]))
                {
                    return new Guard(new Vec2(r, c), ParseDirection(row[c]));
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

    protected override long Part2(string input)
    {
        var charGrid = ParseInput(input);

        // var gridDict = new Dictionary<(int x, int y), IGridItem>();
        var obstructions = LocateObstructions(charGrid).Select(o => o.Position).ToFrozenSet();

        var guard = LocateGuard(charGrid);
        
        var isInGrid = true;

        Dictionary<Vec2, Orientation> visited = [];
        visited.Add(guard.Position, guard.Direction);
        
        var path = new Stack<(Vec2 Position, Orientation Direction)>();
        
        while (isInGrid)
        {
            guard.Move(obstructions);
            
            isInGrid = guard.Position.C < charGrid[0].Length && guard.Position.R < charGrid.Length;

            if (isInGrid)
            {
                if (visited.TryGetValue(guard.Position, out var prevDirections))
                {
                    visited[guard.Position] |= guard.Direction;
                }
                else
                {
                    visited.Add(guard.Position, guard.Direction);
                }
                
                path.Push((guard.Position, guard.Direction));
            }
        }

        Console.WriteLine($"{visited.Count}; {path.Count}; {path.Peek().Position}");

        var options = new HashSet<Vec2>();
        
        foreach (var (position, direction) in path)
        {
            var nextPosition = position.MoveUnit(direction);
            
            // There is already an obstruction
            if (obstructions.Contains(nextPosition)) continue;
            
            var nextPositionWhenBlocked = position.MoveUnit(TurnRight(direction));
            
            if (visited.ContainsKey(nextPositionWhenBlocked) &&
                visited[nextPositionWhenBlocked] == TurnRight(direction))
            {
                options.Add(nextPosition);
            }
        }
        
        Console.WriteLine("options:");
        Console.WriteLine(string.Join("\n", options));

        char[][] newGrid = charGrid.Select(r => r.Select(i => i).ToArray()).ToArray();

        foreach (var (p, o) in visited)
        {
            newGrid[p.R][p.C] = ToChar(o);
        }

        foreach (var option in options)
        {
            char[][] optionGrid = newGrid.Select(r => r.Select(i => i).ToArray()).ToArray();
            
            optionGrid[option.R][option.C] = 'O';
            
            Console.WriteLine($"\nAn option ({option}):");
            PrintGrid(optionGrid);
        }
        

        return options.Count;
    }

    private void PrintGrid(char[][] newGrid)
    {
        for (int r = 0; r < newGrid.Length; r++)
        {
            var row = newGrid[r];
            Console.WriteLine(new string(row));
        }
    }

    private char ToChar(Orientation orientation)
    {
        if (orientation.HasFlag(Orientation.Up) || orientation.HasFlag(Orientation.Down))
        {
            if (orientation.HasFlag(Orientation.Left) || orientation.HasFlag(Orientation.Right))
            {
                return '+';
            }

            return '|';
        }

        return '-';
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