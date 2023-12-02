
using System.Drawing;
using System.Xml;

namespace AoC2022.days;
internal class Day24
{
    public void Run(IEnumerable<string> lines)
    {
        PathPoint start = null;
        PathPoint end = null;
        var blizzards = new Dictionary<PathPoint, char>();
        var lineList = lines.ToList();
        for( var y = 0; y < lineList.Count; y++)
        {
            var l = lineList[y];
            for (var x = 0; x < l.Count(); x++)
            {
                var c = l[x];
                var point = new PathPoint(x, y, 0);
                if (c == '.')
                {
                    if (y == 0)
                    {
                        start = point;
                    }
                    else if (y == lineList.Count - 1)
                    {
                        end = point;
                    }
                }
                else if (c == '>' || c == '<' || c == '^' || c == 'v')
                {
                    blizzards.Add(point, c);
                }
            }
        }

        var maxX = lineList[0].Length - 1;
        var maxY = lineList.Count - 1;

        var blizz = new Blizzards(blizzards.ToLookup(k => k.Key, v => v.Value), new PathPoint(0, 0, 0));
        long minutes = 0;
        for (var i = 0; i < 10000; i++)
        {
            var cheapestPath = FindPath(start, end, blizz, maxX, maxY);
            var cheapestPath2 = FindPath(end, start, cheapestPath.Item2, maxX, maxY);
            blizz = cheapestPath2.Item2;
            minutes += cheapestPath.Item1.Count + cheapestPath2.Item1.Count - 2;
        }
        var cheapestPath3 = FindPath(start, end, blizz, maxX, maxY);

        Console.WriteLine(minutes + cheapestPath3.Item1.Count - 1);
        //Console.WriteLine((cheapestPath.Item1.Count - 1) + ", " + (cheapestPath2.Item1.Count - 1) + ", " + (cheapestPath3.Item1.Count - 1));
        //Console.WriteLine((cheapestPath.Item1.Count - 1) + (cheapestPath2.Item1.Count - 1) + (cheapestPath3.Item1.Count - 1));
    }

    public record Blizzards(ILookup<PathPoint, char> blizzards, PathPoint It);

    public static (List<PathPoint>, Blizzards) FindPath(
            PathPoint from,
            PathPoint to,
            Blizzards startBlizzards,
            long maxX,
            long maxY
        )
    {
        var maxLength = Math.Abs((to.X - from.X)) + Math.Abs((to.Y - from.Y));
        List<PathPoint>? cheapestEnd = null;
        Blizzards cheapestEndBlizzards = null;
        var visitedStates = new Dictionary<string, int>();
        //var cheapest = new Dictionary<PathPoint, WeightedPath>();
        var queue = new PriorityQueue<(List<PathPoint>, PathPoint, Blizzards), long>();
        queue.Enqueue((new List<PathPoint>(), from, startBlizzards), 0);
        while (queue.TryDequeue(out var elem, out var prio))
        {
            var (prevPath, newNode, blizzards) = elem;

            var weight = prevPath.Count + 1;

            if (cheapestEnd is not null && weight + (Math.Abs(to.Y - newNode.Y) + Math.Abs(to.X - newNode.X)) >= cheapestEnd.Count)
            {
                continue;
            }

            var newPath = new List<PathPoint>(prevPath)
            {
                newNode
            };


            if (newNode == to)
            {
                cheapestEnd = newPath;
                cheapestEndBlizzards = blizzards;
                //Console.WriteLine(newPath.Count);
                continue;
            }

            var newBlizzards = UpdateBlizzards(blizzards, maxX, maxY);

            var visitedState = $"{newNode.X};{newNode.Y}|{blizzards.It.X};{blizzards.It.Y}";
            if (visitedStates.TryGetValue(visitedState, out var c) && c <= weight)
            {
                continue;
            }

            visitedStates[visitedState] = weight;


            var neighbours = PathFinding.Neighbours(newNode, false, false)
                .Append(newNode)
                .Where(n => 
                    n == from ||
                    n == to || 
                    (n.X > 0 && n.X < maxX && n.Y > 0 && n.Y < maxY && !newBlizzards.blizzards[n].Any())
                )
                .Select(n => (newPath, n, newBlizzards));

            foreach (var neighbour in neighbours)
            {
                queue.Enqueue(neighbour, newPath.Count + ( maxLength - (maxLength - (Math.Abs(to.Y - neighbour.n.Y) + Math.Abs(to.X - neighbour.n.X)))) * 3);
            }
        }

        return (cheapestEnd, cheapestEndBlizzards);
    }

    private static Dictionary<PathPoint, Blizzards> BlizzardCache = new();

    private static Blizzards UpdateBlizzards(Blizzards blizzards, long maxX, long maxY)
    {
        var nb = new PathPoint((blizzards.It.X + 1) % maxX, (blizzards.It.Y + 1) % maxY, 0);
        if (BlizzardCache.TryGetValue(nb, out var b)) {
            return b;
        }

        var newM = blizzards.blizzards
            .SelectMany(b => b.Select(c => new {
                b.Key,
                Value = c
            }))
            .Select(b =>
            {
                var newPOint = b.Value switch
                {
                    '>' => b.Key with { X = b.Key.X + 1 },
                    'v' => b.Key with { Y = b.Key.Y + 1 },
                    '^' => b.Key with { Y = b.Key.Y - 1 },
                    '<' => b.Key with { X = b.Key.X - 1 },
                };

                if (newPOint.X <= 0)
                {
                    newPOint = newPOint with { X = maxX - 1 };
                }
                else if (newPOint.X >= maxX)
                {
                    newPOint = newPOint with { X = 1 };
                }
                else if (newPOint.Y <= 0)
                {
                    newPOint = newPOint with { Y = maxY - 1 };
                }
                else if (newPOint.Y >= maxY)
                {
                    newPOint = newPOint with { Y = 1 };
                }

                return new {
                    newPOint,
                    b.Value
                };
            })
            .ToLookup(x => x.newPOint, x => x.Value);

        b = new Blizzards(newM, nb);

        BlizzardCache[nb] = b;

        return b;
    }
    internal record VisitedState(PathPoint point, ILookup<PathPoint, char> blizzards);
}
