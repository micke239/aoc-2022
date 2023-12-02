
using System.Security.Cryptography;

namespace AoC2022.days;
internal class Day22
{
    public void Run(IEnumerable<string> lines)
    {
        var map = lines
            .TakeWhile(l => !string.IsNullOrWhiteSpace(l))
            .SelectMany((l, y) =>
            {
                return l.Select((c, x) => new
                {
                    x, y, c
                });
            })
            .Where(v => v.c != ' ')
            .ToDictionary(v => new PathPoint(v.x,v.y,0), v => v.c);

        var instructions = lines.Last();

        //Part1(map, instructions);
        Part2(map, instructions);
    }

    record Fold(PathPoint Folded, PathPoint From, PathPoint To, Fold? Left, Fold? Right, Fold? Up, Fold? Down, int Rotations = 0);

    private void Part2(Dictionary<PathPoint, char> map, string instructions)
    {
        var boundsY = map.Keys
            .GroupBy(p => p.Y)
            .Select(g => new { g.Key, b = (Min: g.Select(p => p.X).Min(), Max: g.Select(p => p.X).Max()) })
            .ToDictionary(v => v.Key, v => v.b);

        var boundsX = map.Keys
            .GroupBy(p => p.X)
            .Select(g => new { g.Key, b = (Min: g.Select(p => p.Y).Min(), Max: g.Select(p => p.Y).Max()) })
            .ToDictionary(v => v.Key, v => v.b);
        
        var maxY = boundsX.Values.Select(y => y.Max).Max();
        var maxX = boundsY.Values.Select(x => x.Max).Max();
        var cubeSize = (maxY + 1 + maxX + 1) / 7;

        var folds = new List<Fold>();

        var startX = boundsY[0].Min;
        var foldY = false;
        for (long y = 0; y < maxY; y+=cubeSize)
        {
            foldY = true;
            var foldAfter = new List<Fold>();
            Fold? firstFold = null;
            for(long x = boundsY[y].Min; x < boundsY[y].Max; x+= cubeSize)
            {
                if (!folds.Any())
                {
                    folds.Add(new Fold(new PathPoint(0, 0, 1), new PathPoint(x, y, 0), new PathPoint(x + (cubeSize - 1), y + (cubeSize - 1), 0), null, null, null, null, 0));
                    foldY = false;
                    //Console.WriteLine(folds.Last());
                    continue;
                }

                var from = new PathPoint(x, y, 0);

                Fold lastFold;
                if (!foldY)
                {
                    lastFold = folds.Last();
                }
                else if (!folds.Any(f => from.X == f.From.X && f.From.Y == y - cubeSize))
                {
                    foldAfter.Add(new Fold(new PathPoint(-1, -1, -1), new PathPoint(x, y, 0), new PathPoint(x + (cubeSize - 1), y + (cubeSize - 1), 0),null,null,null,null));
                    continue;
                }
                else {
                    lastFold = folds.First(f => from.X == f.From.X && f.From.Y == y - cubeSize);
                }

                var fold = FoldIt(lastFold.Folded, foldY ? 0 : 1, foldY ? 1 : 0);

                if (!foldY)
                {
                    var index = folds.IndexOf(lastFold);
                    folds.Add(new Fold(fold, from, new PathPoint(x + (cubeSize - 1), y + (cubeSize - 1), 0), lastFold, null, null, null, lastFold.Rotations + 1));
                    folds[index] = lastFold with { Right = folds.Last() };
                }
                else
                {
                    var index = folds.IndexOf(lastFold);
                    folds.Add(new Fold(fold, from, new PathPoint(x + (cubeSize - 1), y + (cubeSize - 1), 0), null, null, lastFold, null, lastFold.Rotations));
                    folds[index] = lastFold with { Down = folds.Last() };
                }
                
                if (firstFold is null)
                {
                    firstFold = folds.Last();
                }
                
                //Console.WriteLine(folds.Last());
                foldY = false;
            }

            foldAfter.Reverse();

            foreach(var f in foldAfter)
            {
                Fold lastFold;
                if (firstFold is not null)
                {
                    lastFold = firstFold;
                    firstFold = null;
                }
                else
                {
                    lastFold = folds.Last();
                }
                var fold = FoldIt(lastFold.Folded, -1, 0);
                var index = folds.IndexOf(lastFold);
                folds.Add(f with { Folded = fold, Right = lastFold, Rotations = lastFold.Rotations - 1 });
                folds[index] = lastFold with { Left = folds.Last() };
                //Console.WriteLine(folds.Last());
            }
        }

        folds = folds
            .Select(f => Cheat(f, folds, cubeSize))
            .ToList();

        var newMap = map
            .Select(kvp => new { kvp.Key.X, kvp.Key.Y, kvp.Value, Fold = folds.First(f => kvp.Key.Y >= f.From.Y && kvp.Key.X >= f.From.X && kvp.Key.X <= f.To.X && kvp.Key.Y <= f.To.Y) })
            .ToDictionary(x => new PathPoint(x.X, x.Y, 0), x => new { x.Fold, x.Value });

        var dir = 0;
        var pos = new PathPoint(boundsY[0].Min, 0, 0);
        var currentFold = newMap[pos].Fold;
        var path = new Dictionary<PathPoint, int>();
        for (var i = 0; i < instructions.Length; i++)
        {
            var c = instructions[i];
            if (char.IsAsciiDigit(c))
            {
                string nStr = c.ToString();
                while (i < (instructions.Length - 1) && char.IsDigit(instructions[i + 1]))
                {
                    i++;
                    nStr += instructions[i];
                }

                var move = long.Parse(nStr);

                for (var j = 0; j < move; j++)
                {
                    var nPoint = dir switch
                    {
                        0 => new PathPoint(pos.X + 1, pos.Y, 0),
                        1 => new PathPoint(pos.X, pos.Y + 1, 0),
                        2 => new PathPoint(pos.X - 1, pos.Y, 0),
                        3 => new PathPoint(pos.X, pos.Y - 1, 0),
                    };

                    var nDir = dir;
                    if (!newMap.TryGetValue(nPoint, out var p))
                    {
                        (nPoint, nDir) = OutOfBounds(dir, pos, currentFold, cubeSize);
                        p = newMap[nPoint];
                    }

                    if (p.Value == '#')
                    {
                        break;
                    }

                    dir = nDir;
                    pos = nPoint;
                    currentFold = p.Fold;

                    path[pos] = dir;
                    Console.WriteLine(pos + "; " + dir);
                }
            }
            else
            {
                dir = UpdateDir(instructions[i], dir);
            }
        }

        //for(var y = 0; y <= maxY; y++)
        //{
        //    for (var x = 0; x <= maxX; x++)
        //    {
        //        var point = new PathPoint(x, y, 0);
        //        if (path.TryGetValue(point, out var idir))
        //        {
        //            Console.Write(idir switch
        //            {
        //                0 => '>',
        //                1 => 'v',
        //                2 => '<',
        //                3 => '^'
        //            });
        //            continue;
        //        }
                
        //        if (newMap.TryGetValue(point, out var p))
        //        {
        //            Console.Write(p.Value);
        //            continue;
        //        }

        //        Console.Write(" ");
        //    }

        //    Console.WriteLine();
        //}

        Console.WriteLine((pos.X + 1) + ", " + (pos.Y + 1) + ", " + dir);
        Console.WriteLine(1000 * (pos.Y + 1) + 4 * (pos.X + 1) + dir);
    }

    private Fold Cheat(Fold f, List<Fold> folds, long cubeSize)
    {
        if (cubeSize == 4)
        {
            if (f.From == new PathPoint(8,0,0))
            {
                return f with
                {
                    Up = folds.First(f2 => f2.From == new PathPoint(0, 4, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(8, 4, 0)),
                    Right = folds.First(f2 => f2.From == new PathPoint(12, 8, 0)),
                    Left = folds.First(f2 => f2.From == new PathPoint(4, 4, 0)),
                };
            }
            else if (f.From == new PathPoint(0, 4, 0))
            {
                return f with
                {
                    Up = folds.First(f2 => f2.From == new PathPoint(8, 0, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(8, 8, 0)),
                    Right = folds.First(f2 => f2.From == new PathPoint(4, 4, 0)),
                    Left = folds.First(f2 => f2.From == new PathPoint(12, 8, 0)),
                };
            }
            else if (f.From == new PathPoint(4, 4, 0))
            {
                return f with
                {
                    Down = folds.First(f2 => f2.From == new PathPoint(8, 8, 0)),
                    Up = folds.First(f2 => f2.From == new PathPoint(8, 0, 0)),
                };
            }
            else if (f.From == new PathPoint(8, 4, 0))
            {
                return f with
                {
                    Right = folds.First(f2 => f2.From == new PathPoint(12, 8, 0)),
                };
            }
            else if (f.From == new PathPoint(8, 8, 0))
            {
                return f with
                {
                    Left = folds.First(f2 => f2.From == new PathPoint(4, 4, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(0, 4, 0)),
                };
            }
            else
            {
                return f with
                {
                    Up = folds.First(f2 => f2.From == new PathPoint(8, 8, 0)),
                    Right = folds.First(f2 => f2.From == new PathPoint(8, 0, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(0, 4, 0)),
                };
            }
        }
        else
        {
            if (f.From == new PathPoint(50, 0, 0))
            {
                return f with
                {
                    Up = folds.First(f2 => f2.From == new PathPoint(0, 150, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(50, 50, 0)),
                    Left = folds.First(f2 => f2.From == new PathPoint(0, 100, 0)),
                };
            }
            else if (f.From == new PathPoint(100,0, 0))
            {
                return f with
                {
                    Up = folds.First(f2 => f2.From == new PathPoint(0, 150, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(50, 50, 0)),
                    Right = folds.First(f2 => f2.From == new PathPoint(50, 100, 0)),
                };
            }
            else if (f.From == new PathPoint(50, 50, 0))
            {
                return f with
                {
                    Right = folds.First(f2 => f2.From == new PathPoint(100, 0, 0)),
                    Left = folds.First(f2 => f2.From == new PathPoint(0, 100,0))
                };
            }
            else if (f.From == new PathPoint(0, 100, 0))
            {
                return f with
                {
                    Left = folds.First(f2 => f2.From == new PathPoint(50, 0, 0)),
                    Up = folds.First(f2 => f2.From == new PathPoint(50, 50, 0)),
                };
            }
            else if (f.From == new PathPoint(50, 100, 0))
            {
                return f with
                {
                    Right = folds.First(f2 => f2.From == new PathPoint(100, 0, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(0, 150, 0)),
                };
            }
            else
            {
                return f with
                {
                    Right = folds.First(f2 => f2.From == new PathPoint(50, 100, 0)),
                    Left = folds.First(f2 => f2.From == new PathPoint(50, 0, 0)),
                    Down = folds.First(f2 => f2.From == new PathPoint(100, 0, 0)),
                };
            }
        } 


        return null;
    }

    private static int UpdateDir(char with, int dir)
    {
        dir = with switch
        {
            'R' => dir + 1,
            'L' => dir - 1
        };
        if (dir < 0)
        {
            dir = 4 + dir;
        }
        if (dir > 3)
        {
            dir = dir - 4;
        }

        return dir;
    }

    private (PathPoint nPoint, int dir) OutOfBounds(int dir, PathPoint pos, Fold currentFold, long scale)
    {
        var newFold = dir switch
        {
            0 => currentFold.Right,
            1 => currentFold.Down,
            2 => currentFold.Left,
            3 => currentFold.Up
        };

        var xDelta = pos.X - currentFold.From.X;
        var yDelta = pos.Y - currentFold.From.Y;
        var odir = dir;
        var pPos = pos;
        
        (dir, pos) = (scale, (currentFold.From.X, currentFold.From.Y), (newFold.From.X, newFold.From.Y)) switch
        {
            (4, (8, 0), (0, 4)) => (1, new PathPoint(newFold.To.X - xDelta, newFold.From.Y, 0)),
            (4, (8, 0), (4, 4)) => (1, new PathPoint(newFold.From.X + yDelta, newFold.From.Y, 0)),
            (4, (8, 0), (12, 8)) => (2, new PathPoint(newFold.To.X, newFold.From.Y - yDelta, 0)),
            (4, (8, 4), (12, 8)) => (1, new PathPoint(newFold.To.X - yDelta, newFold.From.Y, 0)),
            (4, (8, 8), (0, 4)) => (3, new PathPoint(newFold.To.X - xDelta, newFold.To.Y, 0)),
            (4, (4, 4), (8, 0)) => (0, new PathPoint(newFold.From.X, newFold.To.Y + xDelta, 0)),

            (50, (50, 0), (0, 100)) => (0, new PathPoint(newFold.From.X, newFold.To.Y - yDelta, 0)), // rätt
            (50, (0, 100), (50, 0)) => (0, new PathPoint(newFold.From.X, newFold.To.Y - yDelta, 0)), // rätt
            (50, (50, 100), (0, 150)) => (2, new PathPoint(newFold.To.X, newFold.From.Y + xDelta, 0)), // rätt
            (50, (0, 150), (50, 100)) => (3, new PathPoint(newFold.From.X + yDelta, newFold.To.Y, 0)), // rätt
            (50, (50, 100), (100, 0)) => (2, new PathPoint(newFold.To.X, newFold.To.Y - yDelta, 0)), // rätt
            (50, (100, 0), (50, 100)) => (2, new PathPoint(newFold.To.X, newFold.To.Y - yDelta, 0)), // rätt
            (50, (0, 150), (50, 0)) => (1, new PathPoint(newFold.From.X + yDelta, newFold.From.Y, 0)), // rätt
            (50, (50, 0), (0, 150)) => (0, new PathPoint(newFold.From.X, newFold.From.Y + xDelta, 0)), // rätt
            (50, (100, 0), (0, 150)) => (3, new PathPoint(newFold.From.X + xDelta, newFold.To.Y, 0)), // rätt 2
            (50, (0, 150), (100, 0)) => (1, new PathPoint(newFold.From.X + xDelta, newFold.From.Y, 0)), // rätt 2
            (50, (50, 50), (0, 100)) => (1, new PathPoint(newFold.From.X + yDelta, newFold.From.Y, 0)), // rätt 2
            (50, (0, 100), (50, 50)) => (0, new PathPoint(newFold.From.X, newFold.From.Y + xDelta, 0)), // rätt 2 
            (50, (100, 0), (50, 50)) => (2, new PathPoint(newFold.To.X, newFold.From.Y + xDelta, 0)), // rätt 2
            (50, (50, 50), (100, 0)) => (3, new PathPoint(newFold.From.X + yDelta, newFold.To.Y, 0)), // rätt 2
        };

        //if (dir == 1 || dir == 3) //up or down
        //{
        //    var xchange = ((currentFold.From.X - newFold.From.X) / scale) + (currentFold.Rotations + newFold.Rotations);

        //    if (xchange < 0)
        //    {
        //        for(var i = 0; i < -xchange; i++)
        //        {
        //            dir = UpdateDir('R', dir);
        //        }
        //    }
        //    else if (xchange > 0)
        //    {
        //        for (var i = 0; i < xchange; i++)
        //        {
        //            dir = UpdateDir('L', dir);
        //        }
        //    }

        //    pos = xchange switch
        //    {
        //        -2 or 2 => new PathPoint(newFold.To.X - xDelta, newFold.From.Y + yDelta , 0),
        //        1 => new PathPoint(newFold.From.X + yDelta, newFold.From.Y + xDelta , 0),
        //        -1 => new PathPoint(newFold.From.X + yDelta, newFold.From.Y + xDelta , 0),
        //    };

        //}
        //else //right or left
        //{
        //    var ychange = ((currentFold.From.Y - newFold.From.Y) / scale) + (currentFold.Rotations + newFold.Rotations);
        //    //if (ychange == 3)
        //    //{
        //    //    for (var i = 0; i < 2; i++)
        //    //    {
        //    //        dir = UpdateDir('L', dir);
        //    //    }
        //    //}
        //    if (ychange < 0)
        //    {
        //        for (var i = 0; i < -ychange; i++)
        //        {
        //            dir = UpdateDir('R', dir);
        //        }
        //    }
        //    else if (ychange > 0)
        //    {
        //        for (var i = 0; i < ychange; i++)
        //        {
        //            dir = UpdateDir('L', dir);
        //        }
        //    }

        //    pos = ychange switch
        //    {
        //        -2 or 2 => new PathPoint(newFold.From.X + xDelta, newFold.To.Y - yDelta, 0),
        //        -1 => new PathPoint(newFold.To.X - yDelta, newFold.To.Y - xDelta, 0),
        //        3 => new PathPoint(newFold.From.X + yDelta, newFold.From.Y + xDelta, 0),
        //        1 => new PathPoint(newFold.From.X + yDelta, newFold.To.Y - xDelta, 0)
        //    };
        //}
        
        Console.WriteLine(currentFold.From + " | " + newFold.From + " | " + pPos + " | " + pos + " | " + odir + " | " + dir);

        return (pos, dir);
    }

    private PathPoint FoldIt(PathPoint lastPos, long xFold, long yFold)
    {
        if (yFold == 1)
        {
            return (lastPos.X, lastPos.Y, lastPos.Z) switch
            {
                (-1, 0, 0) => new PathPoint(0, -1, 0),
                (1, 0, 0) => new PathPoint(0, 1, 0),
                (0, -1, 0) => new PathPoint(0, 0, 1),
                (0, 1, 0) => new PathPoint(0, 0, -1),
                (0, 0, -1) => new PathPoint(0, -1, 0),
                (0, 0, 1) => new PathPoint(0, 1, 0),
            };
        }
        else if (xFold == 1)
        {
            return (lastPos.X, lastPos.Y, lastPos.Z) switch
            {
                (-1, 0, 0) => new PathPoint(0, 0, 1),
                (1, 0, 0) => new PathPoint(0, 0, -1),
                (0, -1, 0) => new PathPoint(1, 0, 0),
                (0, 1, 0) => new PathPoint(-1, 0, 0),
                (0, 0, -1) => new PathPoint(1, 0, 0),
                (0, 0, 1) => new PathPoint(1, 0, 0),
            };
        }
        else
        {
            return (lastPos.X, lastPos.Y, lastPos.Z) switch
            {
                (-1, 0, 0) => new PathPoint(0, -1, 0),
                (1, 0, 0) => new PathPoint(0, 0, 1),
                (0, -1, 0) => new PathPoint(1, 0, 0),
                (0, 1, 0) => new PathPoint(-1, 0, 0),
                (0, 0, -1) => new PathPoint(-1, 0, 0),
                (0, 0, 1) => new PathPoint(1, 0, 0),
            };
        }



    }

    /*
     * 
     * 
     * var foldX = (x - startX) / cubeSize;

                var fy = (foldY,  Math.Abs(foldX)) switch
                {
                    (0 or 2,_) => 0,
                    (1, 2) => -1,
                    (3, 2) => 1,
                    (1 or 3, 1) => 0,
                    (3, 0) => -1,
                    (1, 0) => 1,
                };
                var fx = (foldY, foldX) switch
                {
                    (0 or 3, 1 or -3) => 1,
                    (0 or 3, -1 or 3) => -1,
                    (0 or 3, 0 or 2 or -2) => 0,
                    (1 or 2, 1 or -3) => -1,
                    (1 or 2, -1 or 3) => 1,
                    (1 or 2, 0 or 2 or -2) => 0,
                };
                var fz = (Math.Abs(foldX), Math.Abs(foldY)) switch
                {
                    (_, 1 or 3) => 0,
                    (1 or 3, _) => 0,
                    (0, 2) => -1, 
                    (2, 0) => -1, 
                    (0,0) => 1
                };*/

    //private int FindLeftNeighbour(Fold fold, IEnumerable<Fold> folds)
    //{
    //    var indexedFolds = folds.Select((f, idx) => new { f, idx });

    //    var leftNeighbour = indexedFolds.FirstOrDefault(
    //        f => fold.FoldY == f.f.FoldY && fold.FoldX == f.f.FoldX + 1
    //    );
    //    if (leftNeighbour is not null)
    //    {
    //        return leftNeighbour.idx;
    //    }
    //    leftNeighbour = indexedFolds.FirstOrDefault(
    //        f => fold.FoldY == f.f.FoldY - 1 && fold.FoldX == f.f.FoldX + 1
    //    );
    //    if (leftNeighbour is not null)
    //    {
    //        return leftNeighbour.idx;
    //    }
    //    leftNeighbour = indexedFolds.FirstOrDefault(
    //        f => fold.FoldY == f.f.FoldY + 1 && fold.FoldX == f.f.FoldX + 1
    //    );
    //    if (leftNeighbour is not null)
    //    {
    //        return leftNeighbour.idx;
    //    }

    //    return -1;
    //}

    private void Part1(Dictionary<PathPoint, char> map, string instructions)
    {
        var boundsY = map.Keys
            .GroupBy(p => p.Y)
            .Select(g => new { g.Key, b = (Min: g.Select(p => p.X).Min(), Max: g.Select(p => p.X).Max()) })
            .ToDictionary(v => v.Key, v => v.b);

        var boundsX = map.Keys
            .GroupBy(p => p.X)
            .Select(g => new { g.Key, b = (Min: g.Select(p => p.Y).Min(), Max: g.Select(p => p.Y).Max()) })
            .ToDictionary(v => v.Key, v => v.b);

        var pos = new PathPoint(boundsY[0].Min, 0, 0);
        var dir = 0;
        for (var i = 0; i < instructions.Length; i++)
        {
            var c = instructions[i];
            if (char.IsAsciiDigit(c))
            {
                string nStr = c.ToString();
                while (i < (instructions.Length - 1) && char.IsDigit(instructions[i + 1]))
                {
                    i++;
                    nStr += instructions[i];
                }

                var move = long.Parse(nStr);

                for (var j = 0; j < move; j++)
                {
                    var nPoint = dir switch
                    {
                        0 => new PathPoint(pos.X + 1, pos.Y, 0),
                        3 => new PathPoint(pos.X, pos.Y - 1, 0),
                        1 => new PathPoint(pos.X, pos.Y + 1, 0),
                        2 => new PathPoint(pos.X - 1, pos.Y, 0)
                    };

                    if (!map.TryGetValue(nPoint, out var p))
                    {
                        nPoint = dir switch
                        {
                            0 => new PathPoint(boundsY[pos.Y].Min, pos.Y, 0),
                            3 => new PathPoint(pos.X, boundsX[pos.X].Max, 0),
                            1 => new PathPoint(pos.X, boundsX[pos.X].Min, 0),
                            2 => new PathPoint(boundsY[pos.Y].Max, pos.Y, 0)
                        };

                        p = map[nPoint];
                    }

                    if (p == '#')
                    {
                        break;
                    }

                    pos = nPoint;
                }
            }
            else
            {
                dir = instructions[i] switch
                {
                    'R' => dir + 1,
                    'L' => dir - 1
                };
                if (dir < 0)
                {
                    dir = 4 + dir;
                }
                if (dir > 3)
                {
                    dir = dir - 4;
                }
            }
        }

        Console.WriteLine((pos.X + 1) + ", " + (pos.Y + 1) + ", " + dir);
        Console.WriteLine(1000 * (pos.Y + 1) + 4 * (pos.X + 1) + dir);
    }
}
