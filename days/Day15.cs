
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AoC2022.days;
internal class Day15
{
    record Line(Point from, Point to);
    record Point(long x, long y);
    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();
        var max = 4000000;
        var points = lines.SelectMany(l =>
        {
            var list = new List<(Point p, (int d, string t))>();
            var match = Regex.Match(l, "Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)");
            var sensorX = int.Parse(match.Groups[1].Value);
            var sensorY = int.Parse(match.Groups[2].Value);
            var beaconX = int.Parse(match.Groups[3].Value);
            var beaconY = int.Parse(match.Groups[4].Value);

            var sensor = new Point(sensorX, sensorY);
            var distance = Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY);
            list.Add((sensor, (distance, "s")));
            list.Add((new Point(beaconX, beaconY), (0, "b")));

            //for (var x = Math.Max(sensorX - distance, 0); x <= Math.Min(sensorX + distance, max); x++)
            //{
            //    for (var y = Math.Max(sensorY - (distance + Math.Abs(x - sensor.x)), 0); y <= Math.Min(sensorY + (distance - Math.Abs(x - sensor.x)), max); y++)
            //    {
            //        if (x == sensor.x && y == sensorY)
            //        {
            //            continue;
            //        }

            //        if (Math.Abs(y - sensor.y) + Math.Abs(x - sensor.x) > distance)
            //        {
            //            continue;
            //        }

            //        if (x == beaconX && y == beaconY)
            //        {
            //            continue;
            //        }

            //        list.Add((new Point(x, y), "n"));
            //    }
            //}
            //Console.WriteLine(l);
            return list;
        })
        .DistinctBy(x => x.p)
        .ToDictionary(x => x.p, x => x.Item2);

        //var check = new HashSet<Point>();
        var seasonsors = points.Where(kvp => kvp.Value.t == "s").ToList();
        var foundO = false;
        seasonsors.AsParallel().ForAll(s =>
        {
            var otherSeasonsors = seasonsors.Where(si => si.Key != s.Key).ToList();
            var outerRowDistance = s.Value.d + 1;
            for (long y = Math.Max(0, s.Key.y - outerRowDistance); y <= Math.Min(max, s.Key.y + outerRowDistance); y++)
            {
                if (foundO)
                {
                    return;
                }
                var yDist = Math.Abs(y - s.Key.y);
                var rem = outerRowDistance - yDist;
                var fx = s.Key.x - rem;
                var sx = s.Key.x + rem;

                var ps = new List<Point>(2);
                if (fx >= 0)
                {
                    ps.Add(new Point(fx, y));
                }
                if (sx <= max)
                {
                    ps.Add(new Point(sx, y));

                }

                var found = ps.Where(p =>
                {
                    if (points.ContainsKey(p))
                    {
                        return false;
                    }

                    foreach (var s1 in otherSeasonsors)
                    {
                        var old_d = s1.Value.d;
                        var n_d = Math.Abs(s1.Key.x - p.x) + Math.Abs(s1.Key.y - p.y);

                        if (old_d >= n_d)
                        {
                            return false;
                        }
                    }

                    return true;
                }).ToList();

                if (found.Any())
                {
                    Console.WriteLine(found.First().x * 4000000 + y);
                    foundO = true;
                    return;
                }
            }
        });

        //Console.WriteLine(check.Count);

        //for(long y = 0; y <= max; y++)
        //{
        //    var sLines = sesonsors
        //        .Select(s =>
        //        {
        //            var yDist = Math.Abs(s.Key.y - y);
        //            if (yDist > s.Value.d)
        //            {
        //                return null;
        //            }

        //            var start = new Point(Math.Max(0, s.Key.x - (s.Value.d - yDist)), y);
        //            var end = new Point(Math.Min(max, s.Key.x + (s.Value.d - yDist)), y);

        //            return new Line(start, end);
        //        })
        //        .Where(line => line is not null)
        //        .Select(line => line!)
        //        .ToHashSet();

        //    var accLine = sLines.First();
        //    sLines.Remove(accLine);
        //    bool changed = true;
        //    while(changed)
        //    {
        //        changed = false;
        //        foreach(var line in sLines.AsEnumerable())
        //        {
        //            if (line.from.x <= accLine.to.x && line.to.x >= accLine.to.x
        //                || line.to.x >= accLine.from.x && line.from.x <= accLine.from.x)
        //            {
        //                var both = new[] { line, accLine };
        //                var from = both.Select(l => l.from).OrderBy(p => p.x).First();
        //                var to = both.Select(l => l.to).OrderByDescending(p => p.x).First();
        //                accLine = accLine with { from = from, to = to };
        //                sLines.Remove(line);
        //                changed = true;
        //            }
        //            else if (line.from.x >= accLine.from.x && line.to.x <= accLine.to.x)
        //            {
        //                sLines.Remove(line);
        //            }
        //        }
        //    }

        //    if (sLines.Any())
        //    {
        //        //Console.WriteLine("error in " + y);
        //        //Console.WriteLine(accLine);
        //        //Console.WriteLine(string.Join("\n", sLines));
        //        Console.WriteLine((accLine.to.x + 1) * 4000000 + y);
        //        //for (long x = 0; x <= max; x++)
        //        //{
        //        //    if (points.ContainsKey(new Point(x, y)))
        //        //    {
        //        //        continue;
        //        //    }

        //        //    var match = false;
        //        //    foreach (var s in sesonsors)
        //        //    {
        //        //        var old_d = s.Value.d;
        //        //        var n_d = Math.Abs(s.Key.x - x) + Math.Abs(s.Key.y - y);

        //        //        if (old_d >= n_d)
        //        //        {
        //        //            match = true;
        //        //            break;
        //        //        }
        //        //    }
        //        //    if (!match)
        //        //    {
        //        //        break;
        //        //    }
        //        //}
        //        break;
        //    }

        //    //lines.OrderBy(l => l.from.x);


        //    //Console.WriteLine(y);
        //}

        Console.WriteLine(watch.Elapsed);
    }
}
