
using System.Text.Json;

namespace AoC2022.days;
internal class Day14
{
    public void Run(IEnumerable<string> lines)
    {
        var map = lines.
            SelectMany(GetPoints)
            .Distinct()
            .ToDictionary(p => p, p => "#");

        var lowestRock = map.Select(p => p.Key.y).Max() + 2;
        bool done = false;
        int i = 0;
        while (!done)
        {
            var sandP = (500, 0);

            var next = Next(sandP, map, lowestRock);
            if (next is null)
            {
                break;
            }
            while (next is not null)
            {
                if (next.Value.y > lowestRock)
                {
                    done = true;
                    break;
                }
                sandP = next.Value;
                next = Next(sandP, map, lowestRock);
            }

            map[sandP] = "o";
        }

        Console.WriteLine(map.Values.Where(v => v == "o").Count());
    }

    private (int x, int y)? Next((int x, int y) sandP, Dictionary<(int x, int y), string> map, int hy)
    {
        if (sandP.y + 1 == hy)
        {
            return null;
        }

        var p = new[]{
            (sandP.x, sandP.y + 1),
            (sandP.x - 1, sandP.y + 1),
            (sandP.x + 1, sandP.y + 1)
        };

        var np = p.FirstOrDefault(p => !map.ContainsKey(p));

        return np is (0,0) ? null : np;
    }

    private IEnumerable<(int x, int y)> GetPoints(string line)
    {
        var points = line.Split(" -> ")
                    .Select(ToPoint)
                    .ToList();
        for (var i = 0; i + 1 < points.Count; i++)
        {
            var start = points[i];
            var end = points[i + 1];
            if (start.x != end.x)
            {
                if (start.x < end.x)
                {
                    for (int x = start.x; x <= end.x; x++)
                    {
                        yield return (x, start.y);
                    }
                }
                else
                {
                    for (int x = start.x; x >= end.x; x--)
                    {
                        yield return (x, start.y);
                    }
                }
            }
            else
            {
                if (start.y < end.y)
                {
                    for (int y = start.y; y <= end.y; y++)
                    {
                        yield return (start.x, y);
                    }
                }
                else
                {
                    for (int y = start.y; y >= end.y; y--)
                    {
                        yield return (start.x, y);
                    }
                }
            }
        }
    }

    private (int x, int y) ToPoint(string arg1)
    {
        var s = arg1.Split(",");
        return (int.Parse(s[0]), int.Parse(s[1]));
    }
}
