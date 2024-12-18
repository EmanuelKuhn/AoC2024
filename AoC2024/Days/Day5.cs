using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;

namespace AoC2024;

public class Day5() : AoCDay(day: 5, hasTwoInputs: false)
{
    protected override long Part1(string input)
    {
        var (rules, updates) = ParseInput(input);
        
        // Dict of key => all values should be before key
        var rulesDict = rules.GroupBy(r => r.Y, r => r.X).ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToFrozenSet());
        
        var middlePages = new List<int>();

        foreach (var update in updates)
        {
            if (IsCorrect(update, rulesDict))
            {
                var middlePage = update[update.Length / 2];
                
                middlePages.Add(middlePage);
            }
        }
        
        return middlePages.Sum();
    }

    private bool IsCorrect(ImmutableArray<int> update, ImmutableDictionary<int, FrozenSet<int>> rules)
    {
        var seen = new HashSet<int>();

        foreach (var page in update)
        {
            seen.Add(page);
            
            if (rules.TryGetValue(page, out var frozenSet))
            {
                var filteredSet = frozenSet.Intersect(update);
                
                if (!seen.IsSupersetOf(filteredSet))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private record Rule(int X, int Y);

    private static (List<Rule>, List<ImmutableArray<int>>) ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        var rules = new List<Rule>();
        var updates = new List<ImmutableArray<int>>();
        
        foreach (var line in lines)
        {
            if (line.Contains('|'))
            {
                var parts = line.Split("|", StringSplitOptions.RemoveEmptyEntries);
                rules.Add(new Rule(int.Parse(parts[0]), int.Parse(parts[1])));
            } else if (line.Contains(','))
            {
                var parts = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                
                updates.Add([..parts.Select(int.Parse)]);
            }
        }
        
        return (rules, updates);
    }
    
    protected override long Part2(string input)
    {
        var (rules, updates) = ParseInput(input);
        
        // Dict of key => all values should be before key
        var rulesDict = rules.GroupBy(r => r.Y, r => r.X).ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToFrozenSet());
        
        var incorrectUpdates = updates.Where(u => !IsCorrect(u, rulesDict)).ToList();

        var middlePages = incorrectUpdates
            .Select(update => CorrectUpdate(update, rulesDict))
            .Select(corrected => corrected[corrected.Length / 2])
            .ToList();

        return middlePages.Sum();

        
    }

    private ImmutableArray<int> CorrectUpdate(ImmutableArray<int> update, ImmutableDictionary<int, FrozenSet<int>> rules)
    {
        var corrected = new List<int>();
        
        foreach (var page in update)
        {
            if (corrected.Contains(page)) continue;
            
            if (rules.TryGetValue(page, out var frozenSet))
            {
                var filteredSet = frozenSet.Intersect(update).Except(corrected);

                var orderedInserts = filteredSet.OrderBy(update.IndexOf).ToImmutableArray();
                
                orderedInserts = FindInsertOrder(rules, orderedInserts);
                
                corrected.AddRange(orderedInserts);
            }
            
            corrected.Add(page);
        }

        Trace.Assert(corrected.Count == update.Length, $"{corrected.Count}, {update.Length}");
        
        return [..corrected];
    }

    private ImmutableArray<int> FindInsertOrder(ImmutableDictionary<int, FrozenSet<int>> rules, ImmutableArray<int> orderedInserts)
    {
        var converged = false;
        while (!converged)
        {
            var correctedAgain = CorrectUpdate(orderedInserts, rules);

            if (!correctedAgain.SequenceEqual(orderedInserts))
            {
                orderedInserts = correctedAgain;
            }
            else
            {
                converged = true;
            }
        }

        return orderedInserts;
    }


    [Test]
    [TestCase(1, 143)]
    [TestCase(2, 123)]
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