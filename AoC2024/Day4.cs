namespace AoC2024;

public class Day4() : AoCDay(day: 4, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        return 0;
    }
    
    protected override long Part2(string input)
    {
        return 0;
    }
    
    [Test]
    [TestCase(1, 161)]
    [TestCase(2, 48)]
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