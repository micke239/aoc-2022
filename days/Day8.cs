namespace AoC2022.days;
internal class Day8
{
    public void Run(IEnumerable<string> lines)
    {
       var trees = lines
            .Select(l => l.Select(c => Convert.ToInt32(c)).ToArray())
            .ToArray();

        var visibleCount = 0;
        var scores = new List<int>();
        for (var i = 0; i < trees.Length; i++)
        {
            for (var j = 0; j < trees[i].Length; j++)
            {
                var score = ScenicScore(trees, i, j);
                scores.Add(score);
            }
        }

        Console.WriteLine(scores.Max());
    }

    private int ScenicScore(int[][] trees, int y, int x)
    {
        int viewUp = 0;
        for(var i = y-1; i >= 0; i--) //uppåt
        {
            viewUp++;
            if (trees[y][x] <= trees[i][x])
            {
                break;
            }
        }


        int viewDown = 0;
        for (var i = y+1; i < trees.Length; i++) //nedåt
        {
            viewDown++;
            if (trees[y][x] <= trees[i][x])
            {
                break;
            }
        }

        int viewLeft = 0;
        for (var i = x-1; i >= 0; i--) //vänster
        {
            viewLeft++;
            if (trees[y][x] <= trees[y][i])
            {
                break;
            }
        }

        int viewRight = 0;
        for (var i = x+1; i < trees[y].Length; i++) //höger
        {
            viewRight++;
            if (trees[y][x] <= trees[y][i])
            {
                break;
            }
        }

        return viewUp * viewDown * viewLeft * viewRight;
    }
}
