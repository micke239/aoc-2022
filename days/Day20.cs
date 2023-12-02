
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static AoC2022.days.Day16;

namespace AoC2022.days;
internal class Day20
{
    record IdInt(int idx, long value);
    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();
        var multi = 811589153;
        var lineCount = lines.Count();
        var list = lines
            .Select((l,idx) => 
            {
                return new IdInt(idx, long.Parse(l) * multi);
            })
            .ToList();

        var list2 = new List<IdInt>(list);
        for(var x = 0; x < 10; x++)
        {
           Console.WriteLine(string.Join(", ", list2.Select(x => x.value)));
           for (var i = 0; i < list.Count; i++)
           {
                var idx2 = list2.FindIndex(x => x.idx == i);
                long nIndex = list[i].value + idx2;

                if (nIndex < 0)
                {
                    var f = (-nIndex / (list.Count - 1)) + 1;
                    nIndex = nIndex + f * (list.Count - 1);
                }

                if (nIndex >= list.Count)
                {
                    var f = nIndex / (list.Count - 1);
                    nIndex = nIndex - f * (list.Count - 1);
                }

                //if (idx2 > nIndex)
                //{
                //    nIndex += 1;
                //}

                list2.RemoveAt(idx2);
                if (nIndex > 0 || idx2 == 0 /*|| list[i].value > 0*/)
                {
                    list2.Insert((int)nIndex, list[i]);
                }
                else
                {
                    list2.Add(list[i]);
                }
            }
            Console.WriteLine(string.Join(", ", list2.Select(x => x.value)));

            Console.WriteLine(x);
            //list = list2.Select((l, idx) => new IdInt(idx, l.value)).ToList();
            var zeroIndex = list2.FindIndex(l => l.value == 0);
            Console.WriteLine(list2[(zeroIndex + 1000) % list.Count].value + "; " + list2[(zeroIndex + 2000) % list.Count].value + "; " + list2[(zeroIndex + 3000) % list.Count].value) ;
            Console.WriteLine(list2[(zeroIndex + 1000) % list.Count].value + list2[(zeroIndex + 2000) % list.Count].value + list2[(zeroIndex + 3000) % list.Count].value) ;
        }

        Console.WriteLine(watch.Elapsed);
    }


    // for(var x = 0; x< 1; x++)
    //    {
    //       var list2 = new List<IdInt>(list);
    //       for (var i = 0; i<list.Count; i++)
    //       {
    //            //Console.WriteLine(string.Join(", ", list2.Select(x => x.value)));
    //            var idx2 = list2.FindIndex(x => x.idx == i);
    //long nIndex = list[i].value + idx2;

    //            //if (list[i].value < 0)
    //            //{
    //            //    nIndex -= 1;
    //            //}

    //            //if (nIndex < 0)
    //            //{
    //            //    var f = (-nIndex / list.Count) + 1;
    //            //    nIndex = nIndex + f * list.Count;
    //            //}

    //            //if (nIndex >= list.Count)
    //            //{
    //            //    var f = nIndex / (list.Count - 1);
    //            //    nIndex = nIndex - f * (list.Count - 1);
    //            //}

    //            while (nIndex< 0)
    //            {
    //                nIndex += (list.Count - 1);
    //            }

    //            while (nIndex >= list.Count)
    //            {
    //                nIndex -= (list.Count - 1);
    //            }

    //            list2.RemoveAt(idx2);
    //        if (nIndex > 0)
    //        {
    //            list2.Insert((int)nIndex, list[i]);
    //        }
    //        else
    //        {
    //            list2.Add(list[i]);
    //        }
    //                    }
    //                    Console.WriteLine(string.Join(", ", list2.Select(x => x.value)));

    //        Console.WriteLine(x);
    //        list = list2.Select((l, idx) => new IdInt(idx, l.value)).ToList();
    //        var zeroIndex = list2.FindIndex(l => l.value == 0);
    //        Console.WriteLine(list2[(zeroIndex + 1000) % list.Count].value + "; " + list2[(zeroIndex + 2000) % list.Count].value + "; " + list2[(zeroIndex + 3000) % list.Count].value);
    //        Console.WriteLine(list2[(zeroIndex + 1000) % list.Count].value + list2[(zeroIndex + 2000) % list.Count].value + list2[(zeroIndex + 3000) % list.Count].value);
    //    }

}
