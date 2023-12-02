namespace AoC2022.days;
internal class Day4
{
    public void Run(IEnumerable<string> lines)
    {
        var x = lines.Select(l =>
        {
            var s = l.Split(",");
            var first = s[0].Split("-");
            var second = s[1].Split("-");

            var frange = Enumerable.Range(int.Parse(first[0]), int.Parse(first[1]) - int.Parse(first[0]) + 1).ToHashSet();
            var srange = Enumerable.Range(int.Parse(second[0]), int.Parse(second[1]) - int.Parse(second[0]) + 1).ToHashSet();

            return (frange, srange);
        })
        .Where(r1 => {
            var copy1 = r1.frange.ToHashSet();

            var h = copy1.Intersect(r1.srange);

            return h.Count() != 0;
        })
        .Count();

        Console.WriteLine(x);
    }
}
