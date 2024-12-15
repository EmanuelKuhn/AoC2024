using System.Diagnostics;

namespace AoC2024;

public class Day14() : AoCDay(day: 14, hasTwoInputs: false)
{
    private class Robot(Vec2 position, Vec2 velocity)
    {
        public Vec2 Position { get; set; } = position;
        public Vec2 Velocity { get; } = velocity;
    }

    private class World
    {
        private readonly List<Robot> _robots = [];

        private Vec2 WorldSize { get; init; }
        
        public World(List<Robot> robots, Vec2 worldSize)
        {
            WorldSize = worldSize;
            
            _robots = robots;
        }

        private void Step()
        {
            foreach (var robot in _robots)
            {
                var newPosition = robot.Position + robot.Velocity;

                robot.Position = new Vec2(
                    R: newPosition.R >= 0 ? newPosition.R % WorldSize.R : newPosition.R + WorldSize.R,
                    C: newPosition.C >= 0 ? newPosition.C % WorldSize.C : newPosition.C + WorldSize.C);

                Trace.Assert(robot.Position.IsInGrid(WorldSize));
            }
        }

        public void Run(int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                Step();;
            }
        }

        public long ComputeSafetyFactor()
        {
            var middleR = WorldSize.R / 2;
            var middleC = WorldSize.C / 2;
            
            var q1 = _robots.Count(r => r.Position.R < middleR && r.Position.C < middleC);
            var q2 = _robots.Count(r => r.Position.R > middleR && r.Position.C < middleC);
            var q3 = _robots.Count(r => r.Position.R < middleR && r.Position.C > middleC);
            var q4 = _robots.Count(r => r.Position.R > middleR && r.Position.C > middleC);
            
            return q1 * q2 * q3 * q4;
        }
    }

    protected override long Part1(string input)
    {
        return Part1(input, false);
    }
    
    static long Part1(string input, bool isTest)
    {
        Vec2 worldSize = isTest ? new Vec2(7, 11) : new Vec2(103, 101);
        
        var robots = ParseInput(input).ToList();
        
        var world = new World(robots, worldSize);
        
        world.Run(100);
        
        return world.ComputeSafetyFactor();
    }
    
    protected override long Part2(string input)
    {
        return 0;
    }

    private static List<Robot> ParseInput(string input)
    {
        return input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseRobot)
            .ToList();
    }

    private static Robot ParseRobot(string line)
    {
        var pos = line.Split(' ').First().Split('=').Last().Split(',').Select(int.Parse).ToList();
        var v = line.Split("v=").Last().Split(',').Select(int.Parse).ToList();
        
        return new Robot(new Vec2(pos[1], pos[0]), new Vec2(v[1], v[0]));
    }
    
    [Test]
    [TestCase(1, 12)]
    [TestCase(2, 0)]
    public void Example(int part, long expected)
    {
        var result = part switch
        {
            1 => Part1(ExamplePart1, true),
            2 => Part2(ExamplePart2),
            _ => throw new ArgumentOutOfRangeException(nameof(part)),
        };

        Assert.That(result, Is.EqualTo(expected));
    }
}