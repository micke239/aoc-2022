
using System.Diagnostics;

namespace AoC2022.days;
internal class Day18
{
    //record Point(long x, long y, long z);
    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();

        var points =
            lines.Select(l =>
        {
            var s = l.Split(",");
            return new PathPoint(long.Parse(s[0]), long.Parse(s[1]), long.Parse(s[2]));
        })
        .ToHashSet();

        var airpockets = points
            .SelectMany(p => PathFinding.Neighbours(p, false, true).Where(p => !points.Contains(p)))
            .ToList();

        Console.WriteLine(airpockets.Count);

        var cache = new Dictionary<PathPoint, bool>();
        var unblockedAir = airpockets.Where(p => {
            if (cache.TryGetValue(p, out var result))
            {
                return result;
            }
            var path = PathFinding.FindPath(
                p,
                p1 => p1 == new PathPoint(0, 0, 0) || (cache.TryGetValue(p1, out var b) ? b : false),
                p1 => 1,
                (p1, p2) => points.Contains(p2)
                , false, true
            );

            if (path is not null)
            {
                foreach(var ip in path.Path)
                {
                    cache[ip] = true;
                }
                cache[p] = true;
                return true;
            }

            cache[p] = false;
            return false;
        }).ToList();

        Console.WriteLine(unblockedAir.Count);

        Console.WriteLine(watch.Elapsed);
    }
}
