
using System.Diagnostics;

namespace AoC2022.days;
internal class Day12
{
    record Poi(int Value, bool End, bool Start);
    public void Run(IEnumerable<string> lines)
    {
        var map = lines
            .SelectMany((l, y) => l.Select((c, x) => new { y = (long)y, x = (long)x, l = c == 'S' ? 0 : (c == 'E' ? 'z' - 'a' : c - 'a'), Start = c == 'S', End = c == 'E' }))
            .ToDictionary(x => (x.x,x.y), x => new Poi (x.l, x.End,x.Start));

        //var starts = map.Where(kvp => (kvp.Key.x == 0 || kvp.Key.y == 0) && kvp.Value.Value == 0).Select(kvp => kvp.Key);

        //var weight = starts.Select(start => {
        //    var bestTo = new Dictionary<(int, int), int>();
        //    var w = WalkFast(start, 0, map, new HashSet<(int, int)>(), bestTo);
        //    Console.WriteLine(w?.Count);
        //    return w;
        //})
        //.Where(s => s is not null)
        //.OrderBy(s => s.Count)
        //.FirstOrDefault();

        var start = map.Where(kvp => kvp.Value.Start).Select(kvp => kvp.Key).First();
        var end = map.Where(kvp => kvp.Value.End).Select(kvp => kvp.Key).First();

        var w = new Stopwatch();
        w.Start();

        var path = PathFinding.FindPath(
            new PathPoint(start.x, start.y, 0),
            p => map[(p.X, p.Y)].End,
            _ => 1,
            (from, to) => !map.ContainsKey((to.X, to.Y)) || map[(to.X, to.Y)].Value - map[(from.X, from.Y)].Value > 1,
            false,
            false
        );

        Console.WriteLine(w.Elapsed);

        w.Restart();

        var path2 = WalkFast(start, 0, map, new(), new());

        Console.WriteLine(w.Elapsed);

        //var path = WalkFast(start, 0, map, new HashSet<(int, int)>(), new Dictionary<(int, int), int>());

        Console.WriteLine(path.Weight);

        long x = 0, y = 0;
        var hashPath = path.Path.Select(p => (p.X,p.Y)).ToHashSet();
        foreach(var line in lines)
        {
            foreach(var c in line)
            {
                if (hashPath.Contains((x, y)) && path2!.Contains((x, y)))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }
                else if (hashPath.Contains((x,y))) 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (path2!.Contains((x,y)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(c);
                Console.ResetColor();
                x++;
            }
            y++;
            x = 0;
            Console.WriteLine();
        }
    }

    private HashSet<(long,long)>? WalkFast((long x, long y) start, int currentHeight, Dictionary<(long,long), Poi> map, HashSet<(long,long)> preSteps, Dictionary<(long, long), int> bestTo)
    {
        if (map[(start.x,start.y)].End) {
            return preSteps;
        }

        var path =  GetPotentialNeighbours(start.x, start.y)
            .Where(map.ContainsKey)
            .Where(n => !preSteps.Contains(n))
            .Where(n => !bestTo.ContainsKey(n) || bestTo[n] > preSteps.Count)
            .Where(n => map[n].Value - currentHeight < 2)
            .Select(n => {
                if (bestTo.ContainsKey(n))
                {
                    bestTo[n] = preSteps.Count;
                }
                else
                {
                    bestTo.Add(n, preSteps.Count);
                }

                var hash = new HashSet<(long, long)>(preSteps)
                {
                    start
                };

                return WalkFast(n, map[n].Value, map, hash, bestTo);
            })
            .Where(s => s is not null)
            .OrderBy(s => s.Count)
            .FirstOrDefault();

        return path;
    }

    private IEnumerable<(long x, long y)> GetPotentialNeighbours(long x, long y)
    {
        //yield return (x + 1, y + 1);
        yield return (x + 1, y);
        //yield return (x + 1, y - 1);
        yield return (x, y + 1);
        yield return (x, y - 1);
        //yield return (x - 1, y + 1);
        yield return (x - 1, y);
        //yield return (x - 1, y - 1);
    }
}
