namespace AoC2024;

public class Day9() : AoCDay(day: 9, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var diskMap = ParseInput(input);
        
        var disk = FillDisk(diskMap);

        disk = CompactDisk(disk);
        
        return CheckSum(disk);
    }

    private long CheckSum(int?[] disk)
    {
        return disk.Select((fileId, block) => fileId.HasValue ? (long)(fileId.Value * block): 0).Sum();
    }

    private int?[] CompactDisk(int?[] disk)
    {
        var compacted = disk.ToArray();
        
        var endPointer = disk.Length - 1;
        var currentPointer = 0;

        while (currentPointer < endPointer)
        {
            while (compacted[currentPointer].HasValue)
            {
                currentPointer++;
            }
            
            if (currentPointer >= endPointer) break;

            compacted[currentPointer] = compacted[endPointer];
            compacted[endPointer] = null;
            endPointer--;
        }
        
        return compacted;
    }

    private static string DiskToString(IEnumerable<int?> disk)
    {
        return string.Join("", disk.Select(i => i.HasValue ? i.Value.ToString() : "."));
    }

    private static int?[] FillDisk(int[] diskMap)
    {
        var totalDiskSize = diskMap.Sum();

        var disk = new int?[totalDiskSize];

        var fileId = 0;
        var isEmpty = false;

        var block = 0;
        
        foreach (var entry in diskMap)
        {
            var startBlock = block;

            while (block < startBlock + entry)
            {
                disk[block] = isEmpty ? null : fileId;
                block++;
            }

            if (!isEmpty)
            {
                fileId++;
            }
            isEmpty = !isEmpty;
        }

        return disk;
    }


    protected override long Part2(string input)
    {
        var diskMap = ParseInput(input);
        
        var disk = FillDisk(diskMap);

        disk = CompactDiskFileWise(disk);
        
        return CheckSum(disk);    }

    private int?[] CompactDiskFileWise(int?[] disk)
    {
        var compacted = disk.ToArray().AsSpan();
        var freeBlocks = ScanFreeSpace(compacted);

        var fileId = disk.Max().Value;

        // A for loop might have been easier...
        while (fileId > 0)
        {
            // Console.WriteLine(DiskToString(compacted.ToArray()));
            
            var fileStart = disk.ToList().IndexOf(fileId);
            var fileEnd = disk.ToList().LastIndexOf(fileId);

            if (fileStart == -1)
            {
                fileId--;
                continue;
            }
            
            var fileLength = fileEnd - fileStart + 1;

            var firstCandidate = freeBlocks.Where(t => (t.end - t.start) >= fileLength).Select(t => (int?)t.start).FirstOrDefault();

            if (firstCandidate.HasValue)
            {
                var destination = compacted.Slice(firstCandidate.Value, fileLength);
                var source = compacted.Slice(fileStart, fileLength);
                
                if (firstCandidate.Value >= fileStart)
                {
                    fileId--;
                    continue;
                };
                
                source.CopyTo(destination);
                source.Clear();
                freeBlocks.RemoveAll(m => m.start == firstCandidate.Value);
                
                // TODO: just only partly remove a free block if it's partly used instead of rescannig...
                freeBlocks = ScanFreeSpace(compacted);
            }
            
            fileId--;
        }
        
        return compacted.ToArray();
    }

    private static List<(int start, int end)> ScanFreeSpace(ReadOnlySpan<int?> disk)
    {
        List<(int start, int end)> freeBlocks = [];
        
        var currentPointer = 0;
        while (currentPointer < disk.Length)
        {
            while (currentPointer < disk.Length && disk[currentPointer].HasValue) currentPointer++;
            if (currentPointer >= disk.Length) break;
            
            var startBlock = currentPointer;
            
            while (currentPointer < disk.Length && !disk[currentPointer].HasValue) currentPointer++;
            if (currentPointer >= disk.Length) break;
            
            var endBlock = currentPointer;
            
            freeBlocks.Add((startBlock, endBlock));
        }

        return freeBlocks;
    }

    private static int[] ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return lines.First().Select(c => int.Parse(c.ToString())).ToArray();
    }

    [Test]
    public void TestFillDisk()
    {
        var input = ParseInput(ExamplePart1);
        var disk = FillDisk(input);

        var printed = DiskToString(disk);

        var expected = "00...111...2...333.44.5555.6666.777.888899";
        
        Assert.That(printed, Is.EqualTo(expected));
    }
    
    [Test]
    public void TestCompactDisk()
    {
        var input = ParseInput(ExamplePart1);
        var disk = FillDisk(input);

        var compacted = CompactDisk(disk);
        
        var printed = DiskToString(compacted);

        var expected = "0099811188827773336446555566..............";
        
        Assert.That(printed, Is.EqualTo(expected));
    }
    
    [Test]
    public void TestCompactDiskFileWise()
    {
        var input = ParseInput(ExamplePart1);
        var disk = FillDisk(input);

        var compacted = CompactDiskFileWise(disk);
        
        var printed = DiskToString(compacted);

        var expected = "00992111777.44.333....5555.6666.....8888..";
        
        Assert.That(printed, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(1, 1928)]
    [TestCase(2, 2858)]
    public void Example(long part, long expected)
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