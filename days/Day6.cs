using System.Text.RegularExpressions;

namespace AoC2022.days;
internal class Day6
{
    public void Run(IEnumerable<string> lines)
    {
        var p = new List<char>();
        var index = 0;
        var line = lines.First();
        foreach(var c in line)
        {
            index++;
            p.Add(c);

            if (p.Count() == 14)
            {
                if (p.Distinct().Count() == 14)
                {
                    break;
                } 
                else
                {
                    p = p.Skip(1).ToList();
                }
            }

        }

        Console.WriteLine(index);
    }
}
