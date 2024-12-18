using System.Diagnostics;
using SkiaSharp;

namespace AoC2024;

public class Day15() : AoCDay(day: 15, hasTwoInputs: false)
{
    private class World
    {
        public char[][] Map { get; }
        
        private Vec2 WorldSize { get; init; }

        private Vec2 RobotPosition => Map.FindFirst('@')!.Value;
        
        public World(char[][] map)
        {
            WorldSize = new Vec2(map.Length, map[0].Length);

            Map = map;
        }

        private void Step(char operation)
        {
            var position = RobotPosition;
            
            var newPosition = position.Moved(operation);

            var valueAtNewPosition = Map.At(newPosition);
            
            // Trivial cases:
            
            // Into a wall
            if (valueAtNewPosition == '#') return;

            // Into free space
            if (valueAtNewPosition == '.')
            {
                MoveRobot(position, newPosition);
                return;
            }
            
            Trace.Assert(valueAtNewPosition == 'O', $"In this case there should be a product next to the robot, but was: {valueAtNewPosition}");
            
            var firstFreePosition = Map.FindFirstFreePositionBeforeWall(newPosition, operation);

            // No free spaces before wall
            if (firstFreePosition is null) return;
            
            MoveBox(newPosition, firstFreePosition.Value);
            MoveRobot(position, newPosition);
        }

        private void MoveRobot(Vec2 from, Vec2 to)
        {
            Trace.Assert(Map[from.R][from.C] == '@');
            
            Map[from.R][from.C] = '.';
            Map[to.R][to.C] = '@';
        }

        private void MoveBox(Vec2 from, Vec2 to)
        {
            Trace.Assert(Map[from.R][from.C] == 'O');
            
            Map[from.R][from.C] = '.';
            Map[to.R][to.C] = 'O';
        }
        
        public void Run(List<char> operations)
        {
            foreach (var operation in operations)
            {
                Step(operation);
            }
        }

        // public void Render(string path)
        // {
        //     using var surface = SKSurface.Create(new SKImageInfo((int)WorldSize.C, (int)WorldSize.R));
        //
        //     var canvas = surface.Canvas;
        //         
        //     canvas.Clear(SKColors.Black);
        //     
        //     foreach (var robot in _robots)
        //     {
        //         canvas.DrawPoint(robot.Position.C, robot.Position.R, SKColors.Goldenrod);
        //     }
        //
        //     using var image = surface.Snapshot();
        //     using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        //     using var stream = File.OpenWrite(path);
        //     data.SaveTo(stream);
        // }
    }
    
    protected override long Part1(string input)
    {
        var (map, operations) = ParseInput(input);

        var world = new World(map);
        
        world.Run(operations);

        var boxes = world.Map.FindAll('O');

        return boxes.Select(b => b.R * 100 + b.C).Sum();
    }
    
    protected override long Part2(string input)
    {
        return 0;
    }

    private static (char[][] map, List<char> ops) ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var gridStrings = new List<string>();
        
        var operations = new List<char>();
        
        foreach (var line in lines)
        {
            if (line.Contains('#'))
            {
                gridStrings.Add(line);
            }
            else
            {
                operations.AddRange(line);
            }
        }
        
        return (gridStrings.Select(l => l.ToCharArray()).ToArray(), operations);
    }
    
    [Test]
    [TestCase(1, 10092)]
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
    
    public static List<Vec2> FindAll<T>(this T[][] grid, T value)
        where T : struct, IEquatable<T>
    {
        var result = new List<Vec2>();
        
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[r].Length; c++)
            {
                if (grid[r][c].Equals(value))
                {
                    result.Add(new Vec2(r, c));
                }
            }
        }

        return result;
    }

    public static T At<T>(this T[][] grid, Vec2 position)
    {
        return grid[position.R][position.C];
    }

    public static Vec2? FindFirstFreePositionBeforeWall(this char[][] grid, Vec2 position, char direction)
    {
        var currentPosition = position;

        while (grid.IsInGrid(currentPosition))
        {
            currentPosition = currentPosition.Moved(direction);

            var currentValue = grid.At(currentPosition);

            if (currentValue == '.')
            {
                return currentPosition;
            }
            
            if (currentValue == '#') break;
        }

        return null;
    }
    
    public static IEnumerable<Vec2> Positions<T>(this T[][] grid)
    {
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
}