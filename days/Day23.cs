
using System.Drawing;

namespace AoC2022.days;
internal class Day23
{
    public List<string> Directions = new List<string> { "N", "S", "W", "E" };
    public void Run(IEnumerable<string> lines)
    {
        var elves = lines
             .SelectMany((line, y) =>
             {
                 var elves = new List<PathPoint>();
                 for(var x = 0; x < line.Length; x++)
                 {
                     if (line[x] == '#')
                     {
                         elves.Add(new PathPoint(x, y, 0));
                     }
                 }
                 return elves;
             })
             .ToDictionary(x => x, x => 0);

        var ki = 0;
        while (true)
        {
            Console.WriteLine(ki  + 1);
            var moves = new List<(PathPoint, PathPoint)>();
            foreach(var elf in elves)
            {
                var neighbours = PathFinding.Neighbours(elf.Key, true, false)
                    .Select((n, idx) =>
                    {
                        return new
                        {
                            n,
                            d = idx switch
                            {
                                0 => "N",
                                1 => "S",
                                2 => "W",
                                3 => "E",
                                4 => "NW",
                                5 => "SW",
                                6 => "NE",
                                7 => "SE",
                            }
                        };
                    })
                    .Where(n => !elves.ContainsKey(n.n))
                    .ToDictionary(n => n.d, n => n.n);

                if (neighbours.Count == 8) continue;

                PathPoint? moveTo = null; 
                for(var i = 0; i < Directions.Count; i++)
                {
                    var ni = (i + ki) % Directions.Count;
                    var direction = Directions[ni];

                    if (direction == "N")
                    {
                        if (neighbours.ContainsKey("N") && neighbours.ContainsKey("NE") && neighbours.ContainsKey("NW"))
                        {
                            moveTo = neighbours["N"];
                            break;
                        }
                    }
                    else if (direction == "S")
                    {
                        if (neighbours.ContainsKey("S") && neighbours.ContainsKey("SE") && neighbours.ContainsKey("SW"))
                        {
                            moveTo = neighbours["S"];
                            break;
                        }
                    }
                    else if (direction == "W")
                    {
                        if (neighbours.ContainsKey("W") && neighbours.ContainsKey("NW") && neighbours.ContainsKey("SW"))
                        {
                            moveTo = neighbours["W"];
                            break;
                        }
                    }
                    else
                    {
                        if (neighbours.ContainsKey("E") && neighbours.ContainsKey("NE") && neighbours.ContainsKey("SE"))
                        {
                            moveTo = neighbours["E"];
                            break;
                        }
                    }
                }

                if (moveTo is not null)
                {
                    moves.Add((elf.Key, moveTo));
                }

                elves[elf.Key] = (elves[elf.Key] + 1) % Directions.Count;
            }

            var uniqueMoves = moves.GroupBy(m => m.Item2).Where(g => g.Count() == 1).Select(g => g.First());

            if (!uniqueMoves.Any())
            {
                break;
            }

            foreach(var move in uniqueMoves)
            {
                var value = elves[move.Item1];
                elves.Remove(move.Item1);
                elves[move.Item2] = value;
            }

            //Render(elves);
            ki++;
        }

        var minX = elves.Keys.Select(x => x.X).Min();
        var maxX = elves.Keys.Select(x => x.X).Max();
        var minY = elves.Keys.Select(x => x.Y).Min();
        var maxY = elves.Keys.Select(x => x.Y).Max();

        var tiles = (maxY - minY + 1) * (maxX - minX + 1);

        Console.WriteLine(tiles - elves.Count);
        Console.WriteLine(ki + 1);
    }

    private void Render(Dictionary<PathPoint, int> elves)
    {
        var minX = elves.Keys.Select(x => x.X).Min();
        var maxX = elves.Keys.Select(x => x.X).Max();
        var minY = elves.Keys.Select(x => x.Y).Min();
        var maxY = elves.Keys.Select(x => x.Y).Max();

        Console.WriteLine();
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (elves.ContainsKey(new PathPoint(x,y,0)))
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }
}
