
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using static AoC2022.days.Day16;

namespace AoC2022.days;
internal class Day16
{
    private static Dictionary<string, Valve> dict = new();

    public record Valve(string Name, int Rate, IEnumerable<string> Neighbours);

    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();

        //
        var valves = lines
            .Select(l =>
            {
                var match = Regex.Match(l, "Valve ([^\\s]+) has flow rate=(\\d+); tunnels? leads? to valves? (.+)");
                var valve = match.Groups[1].Value;
                var rate = int.Parse(match.Groups[2].Value);
                var neighbours = match.Groups[3].Value.Split(", ");

                return new Valve(valve, rate, neighbours);
            });

        valves = valves
            .Select(n => n with
            {
                Neighbours = n.Neighbours.OrderByDescending(n => valves.First(v => v.Name == n).Rate)
            });

        var valveCount = valves.Count();
        var maxRate = valves.Select(v => v.Rate).Max();
        dict = valves.ToDictionary(v => v.Name);
        var valvesWithRate = valves.Where(r => r.Rate != 0).OrderBy(v => v.Name).ToList();

        var maxPath = FindPath3(dict["AA"], valvesWithRate);

        Console.WriteLine(maxPath);
        Console.WriteLine(watch.Elapsed);
        Console.WriteLine(lkm);

        //Console.WriteLine(max[30].Item2);

        //var path = FindPath(
        //    valves.First(),
        //    (vs, prevPath) => vs.open ? (30 - GetMinutes(prevPath)) * vs.Valve.Rate : 0,
        //    v => v.Valve.Neighbours.Select(n => dict[n]),
        //    (p) => GetMinutes(p.Path) >= 30
        //);

        //foreach (var pa in path!.Path)
        //{
        //    Console.WriteLine(pa.Valve.Name + " - " + pa.open);
        //}

    }

    //Dictionary<int, long> max = new();
    //long max2 = 0;
    Dictionary<string, long> max = new();
    public record Path(Valve ToOpen, WeightedPath Wp);
    static long lkm = 0;
    private long FindPath2(Valve myValve, Valve eleValve, int myMinutes, int eleMinutes, long weight, List<Valve> leftToOpen)
    {
        lkm++;
        //if (leftToOpen.Count == 0)
        //{
        //    return weight;
        //}

        //if (myMinutes >= 30 )
        //{
        //    return weight;
        //}

        //var paths = leftToOpen
        //    .Select(toOpen =>
        //    {
        //        var key = myValve.Name + "-" + myMinutes + "-" + toOpen.Name;
        //        if (max.TryGetValue(key, out var w) && w >= weight)
        //        {
        //            return null;
        //        }
        //        max[key] = weight;

        //        return new Path(toOpen, FindPath(valve, toOpen));
        //    })
        //    .Where(p => p is not null)
        //    .ToList();

        //var ltoKey = string.Join("|", leftToOpen.Select(l => l.Name));
        //var key3 = myMinutes + "|" + eleMinutes + "|" + ltoKey;
        //if (max.TryGetValue(key3, out var w2) && w2 >= weight)
        //{
        //    return w2;
        //}

        //var key4 = eleMinutes + "|" + myMinutes + "|" + ltoKey;

        //max[key3] = weight;
        //max[key4] = weight;

        var values = new List<long>();
        for(int i = 0; i < leftToOpen.Count; i++)
        {
            for(int j = 0; j < leftToOpen.Count; j++)
            {
                if (i == j) continue;

                var myToOpen = leftToOpen[i];
                var eleToOpen = leftToOpen[j];

                var key = myValve.Name + "-" + myMinutes + "-" + myToOpen.Name
                    + "-" + eleValve.Name + "-" + eleMinutes + "-" + eleToOpen.Name;
                if (max.TryGetValue(key, out var w) && w >= weight)
                {
                    continue;
                }
                max[key] = weight;
                var key2 = eleValve.Name + "-" + eleMinutes + "-" + eleToOpen.Name +
                    "-" + myValve.Name + "-" + myMinutes + "-" + myToOpen.Name;
                max[key2] = weight;

                var nv = new List<Valve>(leftToOpen);

                var myPath = FindPath(myValve, myToOpen);
                var elePath = FindPath(eleValve, eleToOpen);

                var myTimePassed = myMinutes + myPath.Path.Count;
                var eleTimePassed = eleMinutes + elePath.Path.Count;

                if (myTimePassed >= 30 && eleTimePassed >= 30)
                {
                    values.Add(weight);
                    continue;
                }
                var nWeight = weight;
                var nMyValve = myToOpen;
                var nMyMinutes = myTimePassed;
                var oneRemovedAt = -1;
                if (myTimePassed < 30)
                {
                    nWeight += myToOpen.Rate * (30 - myTimePassed);
                    oneRemovedAt = i;
                    nv.Remove(myToOpen);
                }
                else
                {
                    nMyValve = myValve;
                    nMyMinutes = myMinutes;
                }

                var nEleValve = eleToOpen;
                var nEleMinutes = eleTimePassed;
                if (eleTimePassed < 30)
                {
                    nWeight += eleToOpen.Rate * (30 - eleTimePassed);
                    if (oneRemovedAt != -1 && oneRemovedAt < j)
                    {
                        nv.RemoveAt(j - 1);
                    }
                    else
                    {
                        nv.RemoveAt(j);
                    }
                }
                else
                {
                    nEleValve = eleValve;
                    nEleMinutes = eleTimePassed;
                }

                var value = FindPath2(nMyValve, nEleValve, nMyMinutes, nEleMinutes, nWeight, nv);
                values.Add(value);
            }
        }

        //var values = pathCombos
        //    .Select(pc =>
        //    {
        //        var nv = new List<Valve>(leftToOpen);
        //        nv.Remove(pc.Item1.ToOpen);
        //        nv.Remove(pc.Item2.ToOpen);

        //        var myTimePassed = myMinutes + pc.Item1.Wp.Path.Count;
        //        var eleTimePassed = eleMinutes + pc.Item2.Wp.Path.Count;
        //        if (myTimePassed >= 30 && eleTimePassed >= 30)
        //        {
        //            return weight;
        //        }
        //        var nWeight = weight;
        //        if (myTimePassed < 30)
        //        {
        //            nWeight = nWeight + (pc.Item1.ToOpen.Rate * (30 - myTimePassed));
        //        }
        //        if (eleTimePassed < 30)
        //        {
        //            nWeight = nWeight + (pc.Item2.ToOpen.Rate * (30 - eleTimePassed));
        //        }

        //        return FindPath2(toOpen, myTimePassed, eleTimePassed, nWeight, nv);
        //    });

        //var values = leftToOpen.Select(toOpen =>
        //{
        //    var key = valve.Name + "-" + minute + "-" + toOpen.Name;
        //    if (max.TryGetValue(key, out var w) && w >= weight)
        //    {
        //        return w;
        //    }
        //    max[key] = weight;
        //    var path = FindPath(valve, toOpen);

        //    var nv = new HashSet<Valve>(leftToOpen);
        //    nv.Remove(toOpen);

        //    var timePassed = minute + path.Path.Count;
        //    if (timePassed >= 30)
        //    {
        //        return weight;
        //    }
        //    var nWeight = weight + (toOpen.Rate * (30 - timePassed));

        //    return FindPath2(toOpen, timePassed, nWeight, nv);
        //});

        if (!values.Any())
        {
            return weight;
        }

        return values.Max();
    }

    public record QueueItem(Valve MyValve, Valve EleValve, int MyMinutes, int EleMinutes, long Weight, List<Valve> LeftToOpen, int MaxLeft);

    private long FindPath3(Valve start, List<Valve> lto)
    {
        var queue = new LinkedList<QueueItem>();
        long maximum = 0;
        queue.AddFirst(new QueueItem(start, start, 4, 4, 0, lto, lto.Sum(l => l.Rate)));
        var watched = new Dictionary<string, long>();
        while (queue.First is not null)
        {
            lkm++;
            var item = queue.First.Value;
            queue.RemoveFirst();

            if (maximum < item.Weight)
            {
                maximum = item.Weight;
            }

            if ((item.Weight + ((30 - Math.Min(item.EleMinutes, item.MyMinutes) - (item.LeftToOpen.Count / 2)) * item.MaxLeft)) <= maximum)
            {
                continue;
            }

            var myKeyStart = item.MyValve.Name + "-" + item.MyMinutes;
            var eleKeyStart = item.EleValve.Name + "-" + item.EleMinutes;
            for (int i = 0; i < item.LeftToOpen.Count; i++)
            {
                for (int j = 0; j < item.LeftToOpen.Count; j++)
                {
                    if (i == j) continue;

                    var myToOpen = item.LeftToOpen[i];
                    var eleToOpen = item.LeftToOpen[j];

                    var key = myKeyStart + "-" + myToOpen.Name + "-" + eleKeyStart + "-" + eleToOpen.Name;
                    if (watched.TryGetValue(key, out var w) && w >= item.Weight)
                    {
                        continue;
                    }
                    watched[key] = item.Weight;
                    var key2 = eleKeyStart + "-" + eleToOpen.Name + "-" + myKeyStart + "-" + myToOpen.Name;
                    watched[key2] = item.Weight;

                    var nv = new List<Valve>(item.LeftToOpen);

                    var myPath = FindPath(item.MyValve, myToOpen);
                    var elePath = FindPath(item.EleValve, eleToOpen);

                    var myTimePassed = item.MyMinutes + myPath.Path.Count;
                    var eleTimePassed = item.EleMinutes + elePath.Path.Count;

                    if (myTimePassed >= 30 && eleTimePassed >= 30)
                    {
                        continue;
                    }
                    var nWeight = item.Weight;
                    var nMaxLeft = item.MaxLeft;

                    var nMyValve = myToOpen;
                    var nMyMinutes = myTimePassed;
                    var oneRemoved = false;
                    if (myTimePassed < 30)
                    {
                        nWeight += myToOpen.Rate * (30 - myTimePassed);
                        oneRemoved = true;
                        nv.RemoveAt(i);
                        nMaxLeft -= myToOpen.Rate;
                    }
                    else
                    {
                        nMyValve = item.MyValve;
                        nMyMinutes = item.MyMinutes;
                    }

                    var nEleValve = eleToOpen;
                    var nEleMinutes = eleTimePassed;
                    if (eleTimePassed < 30)
                    {
                        nWeight += eleToOpen.Rate * (30 - eleTimePassed);
                        if (oneRemoved && i < j)
                        {
                            nv.RemoveAt(j - 1);
                        }
                        else
                        {
                            nv.RemoveAt(j);
                        }
                        nMaxLeft -= eleToOpen.Rate;
                    }
                    else
                    {
                        nEleValve = item.EleValve;
                        nEleMinutes = item.EleMinutes;
                    }

                    queue.AddFirst(new QueueItem(nMyValve, nEleValve, nMyMinutes, nEleMinutes, nWeight, nv, nMaxLeft));
                }
            }
        }

        return maximum;
    }

    private static int GetMinutes(IEnumerable<ValveStuff> path)
    {
        return path.Select(vs => vs.open ? 2 : 1).Sum();
    }


    public record ValveStuff(Valve Valve, bool open);


    public record WeightedPath(HashSet<Valve> Path, long Weight);

    static Dictionary<string, WeightedPath> cache = new();
    public static WeightedPath FindPath(
            Valve from,
            Valve to
        )
    {
        var key = from.Name + "-" + to.Name;
        if (cache.TryGetValue(key, out var path))
        {
            return path;
        }

        WeightedPath? cheapestEnd = null;
        var cheapest = new Dictionary<Valve, WeightedPath>();
        var queue = new LinkedList<(HashSet<Valve>, Valve)>();
        queue.AddLast((new HashSet<Valve>(), from));
        while (queue.First is not null)
        {
            var (prevPath, newNode) = queue.First.Value;
            queue.RemoveFirst();

            var weight = prevPath.Count;

            if (cheapest.TryGetValue(newNode, out var value) && weight >= value.Weight)
            {
                continue;
            }

            if (cheapestEnd is not null && weight >= cheapestEnd.Weight)
            {
                continue;
            }

            var newPath = new HashSet<Valve>(prevPath)
                {
                    newNode
                };
            var wp = new WeightedPath(newPath, weight);

            cheapest[newNode] = wp;

            if (newNode.Name == to.Name)
            {
                cheapestEnd = wp;
                continue;
            }

            var neighbours = newNode.Neighbours.Select(n => dict[n])
                .Where(n => !prevPath.Contains(n))
                .Select(n => (newPath, n));

            foreach (var neighbour in neighbours)
            {
                queue.AddLast(neighbour);
            }
        }

        cache[key] = cheapestEnd!;

        return cheapestEnd!;
    }

    //public static WeightedPath? FindPath(
    //        Valve from,
    //        Func<ValveStuff, IEnumerable<ValveStuff>, int> getWeight,
    //        Func<ValveStuff, IEnumerable<Valve>> getNeighbours,
    //        Func<WeightedPath, bool> isEnd
    //    )
    //{
    //    Dictionary<int, (long, WeightedPath)> bestWeight = new Dictionary<int, (long, WeightedPath)>();
    //    var cheapest = new Dictionary<ValveStuff, WeightedPath>();
    //    var queue = new LinkedList<(HashSet<ValveStuff>, long, ValveStuff)>();
    //    var valveStuffs = new[]
    //    {
    //        //new ValveStuff(from, true),
    //        new ValveStuff(from, false),
    //    };

    //    queue.AddLast((new HashSet<ValveStuff>(), -getWeight(valveStuffs[0], new HashSet<ValveStuff>()), valveStuffs[0]));
    //    //queue.AddLast((new List<ValveStuff>(), -getWeight(valveStuffs[1], new HashSet<ValveStuff>()), valveStuffs[1]));
    //    while (queue.First is not null)
    //    {
    //        var (prevPath, prevWeight, newNode) = queue.First.Value;
    //        queue.RemoveFirst();

    //        var weight = prevWeight + getWeight(newNode, prevPath);

    //        //if (cheapest.TryGetValue(newNode, out var value) && weight <= value.Weight)
    //        //{
    //        //    continue;
    //        //}

    //        //if (cheapestEnd is not null && weight >= cheapestEnd.Weight)
    //        //{
    //        //    continue;
    //        //}

    //        var newPath = new HashSet<ValveStuff>(prevPath)
    //        {
    //            newNode
    //        };
    //        var wp = new WeightedPath(newPath, weight);

    //        //if (
    //        //    newPath.Count > 0 && newPath[0].Valve.Name == "AA" && !newPath[0].open
    //        //    && newPath.Count > 1 && newPath[1].Valve.Name == "DD" && newPath[1].open
    //        //    && newPath.Count > 2 && newPath[2].Valve.Name == "CC" && !newPath[2].open
    //        //    //&& newPath.Count > 3 && newPath[3].Valve.Name == "BB" && newPath[3].open
    //        //    //&& newPath.Count > 4 && newPath[4].Valve.Name == "AA" && !newPath[4].open

    //        //    )
    //        //{
    //        //    Console.WriteLine("Hello");
    //        //}

    //        var minutes = GetMinutes(newPath);
    //        if (bestWeight.TryGetValue(minutes, out var best))
    //        {
    //            if (best.Item1 < weight)
    //            {
    //                bestWeight[minutes] = (weight, wp);
    //            }
    //            //else if (best.Item1 > weight + 170)
    //            //{
    //            //    continue;
    //            //}
    //        }
    //        else
    //        {
    //            bestWeight.Add(minutes, (weight, wp));
    //        }

    //        if (minutes > 30)
    //        {
    //            continue;
    //        }

    //        if (bestWeight.TryGetValue(31, out var best30))
    //        {
    //            var pos = Enumerable.Range(minutes, 30 - minutes)
    //                .Select((m, idx) => (30 - m) * 20)
    //                .Sum();
    //            if (best30.Item1 >= weight + pos)
    //            {
    //                continue;
    //            }
    //        }



    //        var neighbours = getNeighbours(newNode)
    //            .OrderByDescending(n => n.Rate)
    //            .SelectMany(n => new[] { new ValveStuff(n, true), new ValveStuff(n, false) })
    //            .Where(n => n.open ? getWeight(n, wp.Path) != 0 : true)
    //            .Where(n => n.open ? !wp.Path.Contains(n) : true);
    //            //.Where(n => wp.Path.Count < 4 || wp.Path[wp.Path.Count - 4] != n);


    //        //if (!neighbours.Any())
    //        //{
    //        //    Console.WriteLine(wp.Path.Select(vs => vs.open ? 2 : 1).Sum());
    //        //}

    //        foreach (var neighbour in neighbours.Select(n => (newPath, weight, n)))
    //        {
    //            queue.AddFirst(neighbour);
    //        }
    //    }

    //    Console.WriteLine(bestWeight[30]);

    //    return bestWeight[31].Item2;
    //}
}
