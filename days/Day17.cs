
using System.Diagnostics;

namespace AoC2022.days;
internal class Day17
{
    private static readonly int Width = 7;
    private static readonly int SpawnFromLeft = 2;
    private static readonly int SpanAboveRock = 3;

    private static readonly string[][] Shapes = new[]
    {
        new string[]
        {
            "####",
        },
        new string[]
        {
            ".#.",
            "#.#",
            ".#.",
        },
        new string[]
        {
            "..#",
            "..#",
            "###",
        },
        new string[]
        {
            "#",
            "#",
            "#",
            "#"
        },
        new string[]
        {
            "##",
            "##",
        },
    };

    record Point(long x, long y);
    record Shape(long Width, long Height, List<Point> Body);
    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();

        var shapes = Shapes.Select(s =>
        {
            var s2 = new List<Point>();
            for (var y = 0; y < s.Length; y++)
            {
                for (var x = 0; x < s[y].Length; x++)
                {
                    if (s[y][x] == '#')
                    {
                        s2.Add(new Point(x, s.Length - 1 - y));
                    }
                }
            }
            return new Shape(s2.Select(s => s.x).Max() + 1, s2.Select(s => s.y).Max() + 1, s2);
        })
        .ToList();

        var l = lines.First();

        var map = new HashSet<Point>();
        long height = 0;
        long otherHeight = 0;

        long ls = 0;

        long[] heightPerX = new long[]
        {
            0,0,0,0,0,0,0
        };

        var d = new Dictionary<string, (long, long)>();
        for(long i = 0; i < 1000000000000; i++)
        {
            var shapeIdx = (int)(i % Shapes.Length);

            var key = string.Join(",", heightPerX.Select(xy => height - xy)) + "|" + shapeIdx + "|" + (int)(ls % l.Length);
                
            if (d.TryGetValue(key, out var h))
            {
                var iDiff = i - h.Item1;
                var hDiff = otherHeight - h.Item2;

                var f = (1000000000000 - i) / iDiff;

                i += iDiff * f;
                otherHeight += hDiff * f;
            }
            else
            {
                d.Add(key, (i, otherHeight));
            }

            var shape = shapes[shapeIdx];
            var startY = height + 3;
            var startX = 2;
            var currP = new Point(startX, startY);

            while (true)
            {
                var c = l[(int)(ls++ % l.Length)];

                var nCurrP = c switch
                {
                    '<' => currP with { x = currP.x - 1 },
                    '>' => currP with { x = currP.x + 1 },
                    _ => throw new NotSupportedException()
                };
               
                if (!HasCollided(map, nCurrP, shape))
                {
                    currP = nCurrP;
                }

                nCurrP = currP with { y = currP.y - 1 };

                if (HasCollided(map, nCurrP, shape))
                {
                    break;
                }

                currP = nCurrP;
            }

            foreach(var b in shape.Body)
            {
                Point nB = new Point(currP.x + b.x, currP.y + b.y);
                map.Add(nB);
                heightPerX[nB.x] = long.Max(heightPerX[nB.x], nB.y + 1);
            }

            var nHeight = long.Max(currP.y + shape.Height, height);
            var heightDiff = nHeight - height;
            height = nHeight;
            otherHeight += heightDiff;
        }

        Console.WriteLine(otherHeight);

        Console.WriteLine(watch.Elapsed);
    }

    private void Render(HashSet<Point> map, long height, HashSet<Point> body)
    {
        for(long y = height; y > height - 6; y--)
        {
            Console.Write("|");
            for(long x = 0; x < 7; x++)
            {
                if (body.Contains(new Point(x,y)))
                {
                    Console.Write("@");
                }
                else if (map.Contains(new Point(x,y)))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write("|");
            Console.WriteLine();
        }
        Console.WriteLine("+-------+");
        Console.WriteLine();
    }

    private static bool HasCollided(HashSet<Point> map, Point currP, Shape shape)
    {
        return shape.Body.Any(b =>
        {
            var position = new Point(currP.x + b.x, currP.y + b.y);

            if (position.x < 0 || position.x > 6)
            {
                return true;
            }

            if (position.y < 0)
            {
                return true;
            }

            return map.Contains(position);
        });
    }
}
