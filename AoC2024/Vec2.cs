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
}