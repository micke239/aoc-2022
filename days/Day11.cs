
using System.Numerics;
using System.Security.Cryptography;

namespace AoC2022.days;
internal class Day11
{
    private Monkey[] monkeys;

    record Worry(int Initial)
    {
        public Dictionary<int,int> Rests { get; set; } = new();
    };
    record Monkey(List<Worry> Items, Action<Worry> Operation, int DivisibleTest, int TrueMonkey, int FalseMonkey);
    public void Run(IEnumerable<string> lines)
    {
        monkeys = lines.Chunk(7)
            .Select(lines =>
            {
                var startingItems = lines[1].Replace("Starting items: ", "").Trim();
                var operation = lines[2].Replace("Operation: new = old", "").Trim();
                var divisibleTest = lines[3].Replace("Test: divisible by ", "").Trim();
                var ifTrueMonkey = lines[4].Replace("If true: throw to monkey ", "").Trim();
                var ifFalseMonkey = lines[5].Replace("If false: throw to monkey ", "").Trim();

                return new Monkey(
                    Items: startingItems.Split(", ").Select(s => new Worry(int.Parse(s))).ToList(),
                    Operation: ParseOperation(operation),
                    DivisibleTest: int.Parse(divisibleTest),
                    TrueMonkey: int.Parse(ifTrueMonkey),
                    FalseMonkey: int.Parse(ifFalseMonkey)
                );
            })
            .ToArray();

        foreach(var item in monkeys.SelectMany(m => m.Items))
        {
            foreach(var divider in monkeys.Select(m => m.DivisibleTest))
            {
                item.Rests.Add(divider, item.Initial % divider);
            }
        }

        var handled = new long[monkeys.Length];
        for(int x = 1; x <= 10000; x++)
        {
            for(int i = 0; i < monkeys.Length; i++)
            {
                var monkey = monkeys[i];
                foreach(var item in monkey.Items)
                {
                    monkey.Operation(item);
                    var newMonkey = monkey.FalseMonkey;
                    if (item.Rests[monkey.DivisibleTest] == 0)
                    {
                        newMonkey = monkey.TrueMonkey;
                    }
                    monkeys[newMonkey].Items.Add(item);
                    handled[i]++;
                }
                monkey.Items.Clear();
            }
            //foreach(var monkey in monkeys)
            //{
            //    Console.WriteLine(monkey.Items.Count);
            //}
            //Console.WriteLine(string.Join(", ", handled));
        }
        var handledS = handled.OrderByDescending(x => x).ToList();
        Console.WriteLine(handledS[0] + " * " + handledS[1]);
        Console.WriteLine(handledS[0] * handledS[1]);
    }

    private Action<Worry> ParseOperation(string operation)
    {
        var op = operation.Split(" ");

        return (op[0], op[1]) switch
        {
            ("*", "old") => old => {
                foreach(var (divider, value) in old.Rests.AsEnumerable())
                {
                    old.Rests[divider] = (old.Rests[divider] * old.Rests[divider]) % divider;
                }
            },
            ("+", "old") => old => {
                foreach (var (divider, value) in old.Rests.AsEnumerable())
                {
                    old.Rests[divider] = (old.Rests[divider] + old.Rests[divider]) % divider;
                }
            },
            ("*", string s) when int.TryParse(s, out var n) => old => {
                foreach (var (divider, value) in old.Rests.AsEnumerable())
                {
                    old.Rests[divider] = (old.Rests[divider] * n) % divider;
                }
            },
            ("+", string s) when int.TryParse(s, out var n) => old => {
                foreach (var (divider, value) in old.Rests.AsEnumerable())
                {
                    old.Rests[divider] = (old.Rests[divider] + n) % divider;
                }
            },
            _ => old => { }
        } ;
    }
}
