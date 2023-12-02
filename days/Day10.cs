namespace AoC2022.days;
internal class Day10
{
    public void Run(IEnumerable<string> lines)
    {
        var currValue = 1;
        var screen = new List<char>();

        foreach (var line in lines)
        {
            var s = line.Split(" ");
            var op = s[0];

            if (op == "noop")
            {
                RenderPixel(screen, currValue);
            }
            else
            {
                RenderPixel(screen, currValue);
                RenderPixel(screen, currValue);
                currValue += int.Parse(s[1]);
            }
        }

        foreach(var row in screen.Chunk(40))
        {
            Console.WriteLine(row);
        }
    }

    private void RenderPixel(List<char> screen, int currValue)
    {
        var pos = screen.Count % 40;
        if (Math.Abs(currValue - pos) <= 1)
        {
            screen.Add('#');
        }
        else
        {
            screen.Add('.');
        }
    }
}
