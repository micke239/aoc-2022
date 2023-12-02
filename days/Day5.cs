using System.Text.RegularExpressions;

namespace AoC2022.days;
internal class Day5
{
    //private static readonly List<List<string>> Stacks = new() {
    //    new() { "Z", "N"},
    //    new() { "M","C","D" },
    //    new() { "P" },
    //};

    //private static readonly List<List<string>> Stacks = new() {
    //    new() { "P","F","M","Q","W","G", "R", "T",},
    //    new() { "H","F","R" },
    //    new() { "P","Z","R","V","G","H","S","D" },
    //    new() { "Q","H","P","B","F","W","G" },
    //    new() { "P","S","M","J","H" },
    //    new() { "M","Z","T","H","S","R","P","L" },
    //    new() { "M","Z","T","H","S","R","P","L" },
    //};

   /// <summary>
   /// [T]     [D]         [L]            
///     R]     [S] [G]     [P]         [H]
///     G]     [H] [W]     [R] [L]     [P]
///     W]     [G] [F] [H] [S] [M]     [L]
///     Q]     [V] [B] [J] [H] [N] [R] [N]
///     M] [R] [R] [P] [M] [T] [H] [Q] [C]
///     F] [F] [Z] [H] [S] [Z] [T] [D] [S]
///     P] [H] [P] [Q] [P] [M] [P] [F] [D]
///     1   2   3   4   5   6   7   8   9 
   /// </summary>
   /// <param name="lines"></param>

    public void Run(IEnumerable<string> lines)
    {
        var stacks = lines
            .TakeWhile(l => l.StartsWith("["))
            .Aggregate(new List<LinkedList<char>>(), (stacks, line) =>
            {
                var items = line
                    .Skip(1)
                    .Chunk(4)
                    .Select((l, index) => (l.First(), index));

                if (!stacks.Any())
                {
                    for (var i = 0; i < items.Count(); i++)
                    {
                        stacks.Add(new LinkedList<char>());
                    }
                }

                foreach (var (item, index) in items.Where(i => i.Item1 != ' '))
                {
                    stacks[index].AddFirst(item);
                }

                return stacks;
            });

        foreach(var instruction in lines.SkipWhile(l => !l.StartsWith("m")))
        {
            var m = Regex.Match(instruction, "move (\\d+) from (\\d+) to (\\d+)");
            var move = int.Parse(m.Groups[1].Value);
            var from = int.Parse(m.Groups[2].Value);
            var to = int.Parse(m.Groups[3].Value);

            var itemsToMove = stacks[from - 1].Skip(stacks[from - 1].Count - move).Take(move).ToList();

            foreach(var item in itemsToMove)
            {
                stacks[to - 1].AddLast(item);
                stacks[from - 1].RemoveLast();
            }
        }

        foreach(var stack in stacks)
        {
            Console.Write(stack.Last());
        }

    }
}
