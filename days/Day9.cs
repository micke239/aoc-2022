namespace AoC2022.days;
internal class Day9
{
    public void Run(IEnumerable<string> lines)
    {
        var visited = new HashSet<(int x, int y)>();
        var head = (x: 0, y: 0);
        var knot1 = (x: 0, y: 0);
        var knot2 = (x: 0, y: 0);
        var knot3 = (x: 0, y: 0);
        var knot4 = (x: 0, y: 0);
        var knot5 = (x: 0, y: 0);
        var knot6 = (x: 0, y: 0);
        var knot7 = (x: 0, y: 0);
        var knot8 = (x: 0, y: 0);
        var tail = (x: 0, y: 0);
        visited.Add(tail);
        foreach (var line in lines)
        {
            var s = line.Split(" ");
            var dir = s[0];
            var count = int.Parse(s[1]);


            //Console.WriteLine($"== {line} ==");

            for(var i = 0; i < count; i++)
            {
                head = dir switch
                {
                    "U" => (head.x, head.y + 1),
                    "D" => (head.x, head.y - 1),
                    "R" => (head.x + 1, head.y),
                    "L" => (head.x - 1, head.y),
                    _ => throw new NotSupportedException()
                };

                knot1 = PositionTail(head, knot1);
                knot2 = PositionTail(knot1, knot2);
                knot3 = PositionTail(knot2, knot3);
                knot4 = PositionTail(knot3, knot4);
                knot5 = PositionTail(knot4, knot5);
                knot6 = PositionTail(knot5, knot6);
                knot7 = PositionTail(knot6, knot7);
                knot8 = PositionTail(knot7, knot8);
                tail = PositionTail(knot8, tail);
                visited.Add(tail);

                //Render(head, tail);
            }

        }

        Console.WriteLine(visited.Count);
    }

    private void Render((int x, int y) head, (int x, int y) tail)
    {
        for(var y = 100; y >= 0; y--)
        {
            for (var x = 0; x < 100; x++)
            {
                if ((x,y) == head)
                {
                    Console.Write("H");
                }
                else if ((x, y) == tail)
                {
                    Console.Write("T");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");
    }

    private (int x, int y) PositionTail((int x, int y) head, (int x, int y) tail)
    {
        var xDist = head.x - tail.x;
        var yDist = head.y - tail.y;

        if (Math.Abs(xDist) < 2 && Math.Abs(yDist) < 2) return tail;

        return (xDist, yDist) switch
        {
            (-2, -2) => (tail.x - 1, tail.y - 1),
            (-2, 2) => (tail.x - 1, tail.y + 1),
            (2, 2) => (tail.x + 1, tail.y + 1),
            (2, -2) => (tail.x + 1, tail.y - 1),

            (-2, -1) => (tail.x - 1,tail.y - 1),
            (-2, 1) => (tail.x - 1, tail.y + 1),
            (2, -1) => (tail.x + 1, tail.y - 1),
            (2, 1) => (tail.x + 1, tail.y + 1),
            (2, 0) => (tail.x + 1, tail.y),
            (-2, 0) => (tail.x - 1, tail.y),

            (-1, -2) => (tail.x - 1, tail.y - 1),
            (1, -2) => (tail.x + 1, tail.y - 1),
            (-1, 2) => (tail.x - 1, tail.y + 1),
            (1, 2) => (tail.x + 1, tail.y + 1),
            (0, 2) => (tail.x, tail.y + 1),
            (0, -2) => (tail.x, tail.y - 1),

            _ => throw new NotSupportedException()
        };
    }
}
