namespace AoC2024;

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
    
    public static char RotatedLeft(this char direction)
    {
        return direction switch
        {
            Up => Left,
            Left => Down,
            Down => Right,
            Right => Up,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}