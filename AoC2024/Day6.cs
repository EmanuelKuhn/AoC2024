using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;

namespace AoC2024;

public class Day6() : AoCDay(day: 6, hasTwoInputs: false)
{
    [Flags]
    private enum Directions
    {
        Up, 
        Down, 
        Left, 
        Right
    }
    
    private Directions ParseDirection(char c)
    {
        return c switch
        {
            '^' => Directions.Up,
            'v' => Directions.Down,
            '<' => Directions.Left,
            '>' => Directions.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }

    private static ImmutableArray<char> _guardSymbols = ['^', 'v', '<', '>'];

    private record Vec2(int R, int C)
    {
        public Vec2 Move(Vec2 v)
        {
            return new Vec2(R + v.R, C + v.C);
        }
        
        
        
        public Vec2 MoveUnit(char direction)
        {
            return Move(AsUnit(direction));
        }
        
        public static Vec2 AsUnit(char direction)
        {
            return direction switch
            {
                '^' => new Vec2(-1, 0),
                'v' => new Vec2(1, 0),
                '<' => new Vec2(0, -1),
                '>' => new Vec2(0, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
    
    private interface IGridItem
    {
        Vec2 Position { get; }
    }

    private class Guard(Vec2 position, char direction) : IGridItem
    {
        public Vec2 Position { get; private set; } = position;
        public char Direction { get; private set; } = direction;

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

        private char TurnRight(char direction)
        {
            return direction switch
            {
                '^' => '>',
                'v' => '<',
                '<' => '^',
                '>' => 'v',
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
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

    protected override long Part2(string input)
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

    [Test]
    [TestCase(1, 41)]
    [TestCase(2, 100)]
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