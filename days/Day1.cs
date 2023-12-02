namespace AoC2022.days;

internal class Day1
{
    public void Run(IEnumerable<string> lines)
    {
        var dict = new Dictionary<int, List<int>>();
        int counter = 0;

        foreach (var line in lines)
        {
            if (line.Trim() == string.Empty)
            {
                counter++;
                continue;
            }

            if (!dict.ContainsKey(counter))
            {
                dict.Add(counter, new List<int>());
            }

            dict[counter].Add(int.Parse(line.Trim()));
        }

        var max = dict.Select(d => d.Value.Sum()).OrderDescending();

        Console.WriteLine(max.Take(3).Sum());
    }
    }