namespace AoC2022.days;
internal class Day3
{
    public void Run(IEnumerable<string> lines)
    {
        var rucksacks = 
            lines.Select(l => l.ToHashSet()).ToList();


        var sum = rucksacks
            .Chunk(3)
            .Select(group => group.First().Intersect(group.Skip(1).First()).Intersect(group.Skip(2).First()))
            .Select(r => {
                Console.WriteLine(string.Join("", r));
                return r;
            })
            .Select(p => {
                if (char.IsLower(p.First()))
                {
                    return p.First() - 'a' + 1;
                }
                else
                {
                    return p.First() - 'A' + 27;
                }
            })
            .Select(i =>
            {
                Console.WriteLine(i);
                return i;
            })
            .Sum();

        Console.Write(sum);
    }
}
