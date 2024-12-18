using System.Numerics;

namespace AoC2024;

public readonly record struct Vec2(long R, long C)
{
    public Vec2 Moved(Vec2 v)
    {
        return new Vec2(R + v.R, C + v.C);
    }
        
    public Vec2 Moved(char direction)
    {
        return Moved(direction.AsUnit());
    }
    
    public static Vec2 operator +(Vec2 a1, Vec2 a2)
    {
        return new Vec2(a1.R + a2.R, a1.C + a2.C);
    }

    public static Vec2 operator -(Vec2 a1, Vec2 a2)
    {
        return new Vec2(a1.R - a2.R, a1.C - a2.C);
    }
    
    public static Vec2 operator *(long a, Vec2 b)
    {
        return new Vec2(a * b.R, a * b.C);
    }
    
    public static implicit operator Vec2((long, long) pair)
    {
        return new Vec2(pair.Item1, pair.Item2);
    }
}

internal static class Vec2Extensions
{
    public static long ManhattanDistance(this Vec2 a, Vec2 b)
    {
        return Math.Abs(a.R - b.R) + Math.Abs(a.C - b.C); 
    }
    
    public static bool IsInGrid(this Vec2 position, Vec2 gridSize)
    {
        return position is { C: >= 0, R: >= 0 } && position.C < gridSize.C && position.R < gridSize.R;
    }

    public static Vec2 Normalized(this Vec2 x)
    {
        // Turned out not to be needed...
        var gcd = (int) BigInteger.GreatestCommonDivisor(x.C, x.R);

        if (gcd == 1) return x;

        return new Vec2(x.R / gcd, x.C / gcd);
    }
    
    public static IEnumerable<Vec2> Neighbours(this Vec2 position, Vec2 gridSize)
    {
        (int, int)[] offsets = [(0, 1), (0, -1), (1, 0), (-1, 0)];
        var coords = offsets.Select(o => new Vec2(position.R + o.Item1, position.C + o.Item2)).ToArray();

        return coords.Where(c => c.IsInGrid(gridSize));
    }
}