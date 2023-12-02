
using System.Text.Json;

namespace AoC2022.days;
internal class Day13
{
    public record Sign(int? Value, List<Sign>? ListValue);
    public void Run(IEnumerable<string> lines)
    {
        var pairs = lines.Chunk(3)
            .Select(l =>
            {
                var pair = new List<Sign>[2];
                var lists = new[]
                {
                    (0, l.First()),
                    (1, l.Skip(1).First()),
                };

                foreach (var (idx, line) in lists)
                {
                    List<Sign> main = null;
                    var stack = new LinkedList<List<Sign>>();
                    string cur = "";
                    foreach (var c in line)
                    {
                        if (c == '[')
                        {
                            var nl = new List<Sign>();
                            if (stack.Last is not null)
                            {
                                stack.Last.Value.Add(new Sign(null, nl));
                            }
                            else
                            {
                                main = nl;
                            }
                            stack.AddLast(nl);
                        }
                        else if (c == ',')
                        {
                            if (!string.IsNullOrEmpty(cur))
                            {
                                stack.Last().Add(new Sign(int.Parse(cur), null));
                            }
                            cur = "";
                        }
                        else if (c == ']')
                        {
                            if (!string.IsNullOrEmpty(cur))
                            {
                                stack.Last().Add(new Sign(int.Parse(cur), null));
                            }
                            cur = "";
                            stack.RemoveLast();
                        }
                        else
                        {
                            cur += c.ToString();
                        }
                    }

                    pair[idx] = main!;
                }

                return pair;
            })
            //.SelectMany(pair => pair)
            //.OrderBy(p => p, Comparer<List<Sign>>.Create((p1,p2) => IsValid(p1, p2) switch {
            //    true => -1,
            //    false => 1,
            //    null => 0
            //}))
            .ToList();

        //var a = pairs
        //    .Select((p,idx) => new {p, idx})
        //    .Where(p => 
        //        p.p.Count == 1 && p.p[0].ListValue is not null && p.p[0].ListValue.Count == 1 && (p.p[0].ListValue[0].Value == 2 || p.p[0].ListValue[0].Value == 6)
        //    )
        //    .ToList();

        //Console.WriteLine((a[0].idx+1) * (a[1].idx+1));

        var validPairs = new List<int>();
        for (var i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];

            if (IsValid(pair[0], pair[1]) ?? false)
            {
                validPairs.Add(i + 1);
            }
        }

        Console.WriteLine(string.Join(", ", validPairs));
        Console.WriteLine(validPairs.Sum());
    }

    private static bool? IsValid(List<Sign> sign1List, List<Sign> sign2List)
    {
        for (var i = 0; i < sign1List.Count; i++)
        {
            if (sign2List.Count <= i)
            {
                return false;
            }

            var sign1 = sign1List[i];
            var sign2 = sign2List[i];

            if (sign1.Value is not null && sign2.Value is not null)
            {
                if (sign1.Value < sign2.Value)
                {
                    return true;
                }
                else if (sign1.Value > sign2.Value)
                {
                    return false;
                }

                continue;
            }
            else if (sign1.ListValue is not null && sign2.ListValue is null)
            {
                var valid =  IsValid(sign1.ListValue, new List<Sign> { sign2 });
                if (valid is not null)
                {
                    return valid;
                }
            }
            else if (sign2.ListValue is not null && sign1.ListValue is null)
            {
                var valid = IsValid(new List<Sign> { sign1 }, sign2.ListValue);
                if (valid is not null)
                {
                    return valid;
                }
            }
            else
            {
                var valid = IsValid(sign1.ListValue!, sign2.ListValue!);
                if (valid is not null)
                {
                    return valid;
                }
            }
        }

        if (sign2List.Count > sign1List.Count)
        {
            return true;
        }
        else if (sign2List.Count < sign1List.Count)
        {
            return false;
        }

        return null;
    }
}
