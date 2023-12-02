using System.Text.RegularExpressions;

namespace AoC2022.days;
internal class Day7
{
    public record File(string Name, int Size);
    public record Directory(string Name, List<File> Files, Dictionary<string, Directory> directories, Directory? Parent);
    public void Run(IEnumerable<string> lines)
    {
        var root = new Directory("", new List<File>(), new Dictionary<string, Directory>(), null);
        var currentDir = root;
        foreach (var line in lines) 
        {
            var arguments = line.Split(" ");
            if (arguments[0] == "$")
            {
                if (arguments[1] == "cd")
                {
                    if (arguments[2] == "..")
                    {
                        currentDir = currentDir.Parent!;
                    }
                    else if (arguments[2] == "/")
                    {
                        currentDir = root;
                    }
                    else
                    {
                        if (!currentDir.directories.ContainsKey(arguments[2]))
                        {
                            currentDir.directories.Add(
                                arguments[2],
                                new Directory(arguments[2], new(), new(), currentDir)
                            );
                        }
                        
                        currentDir = currentDir.directories[arguments[2]];
                    }
                }
                else if (arguments[1] == "ls")
                {

                }
            }
            else
            {
                if (arguments[0] == "dir")
                {
                    continue;
                }

                currentDir.Files.Add(new File(arguments[1], int.Parse(arguments[0])));
            }
        };

        Console.WriteLine("Structure done, calculating..");

        var sum = GetDirectories(root).Append(root)
            .ToDictionary(d => d, d => GetSize(d));

        var spaceNeeded = 30000000 - (70000000 - sum[root]);

        var d2 = sum.OrderBy(d => d.Value)
            .SkipWhile(d => d.Value < spaceNeeded)
            .First();

        Console.WriteLine(d2.Value);

        //foreach(var i in sum)
        //{
        //    if (i.Item1 > 30000000)
        //    {
        //        Console.WriteLine(i.Item1);
        //    }
        //}
    }

    private int GetSize(Directory d)
    {
        return d.Files.Sum(f => f.Size) + d.directories.Values.Sum(d => GetSize(d));
    }

    private IEnumerable<Directory> GetDirectories(Directory directory)
    {
        foreach(var d in directory.directories.Values)
        {
            yield return d;
            foreach (var x in GetDirectories(d))
            {
                yield return x;
            }
        }
    }
}
