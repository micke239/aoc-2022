
using System.Drawing;
using System.Numerics;
using System.Xml;

namespace AoC2022.days;
internal class Day25
{
    public void Run(IEnumerable<string> lines)
    {
        var sum =
            lines.Select(l =>
            {
                long value = 0;
                var length = l.Length;
                for(var i = 0; i < l.Length; i++)
                {
                    var c = l[i];
                    var v = c switch
                    {
                        '2' => 2,
                        '1' => 1,
                        '0' => 0,
                        '-' => -1,
                        '=' => -2
                    };

                    value += ((long)Math.Pow(5, (length - i - 1))) * v; 
                }
                return value;
            }).Sum();

        var xy = (double)sum;
        var divider = 0;
        while (xy > 1)
        {
            xy = xy / 5;
            divider++;
        }

        var length =  divider - 1;
        string v = "";
        var rest = sum;
        for (var i = length; i > -1; i--)
        {
            var x = (long)Math.Pow(5, i);
            var iv = rest / x;


            if (iv == 0 || iv == 1 || iv == 2)
            {
                v += iv;
            }
            else if (iv == 3 || iv == 4) {
                v = Increment(v) + (iv == 3 ? "=" : "-");
            }

            rest = rest - (iv * x);
        }

        Console.WriteLine(sum);
        Console.WriteLine(v);
    }

    private long Pow(int v, long i)
    {
        if (i == 0)
        {
            return 1;
        }

        long v1 = v;
        for (var idx = 1; idx < i; idx++)
        {
            v1 *= v1;
        }
        return v1;
    }

    private string Increment(string v)
    {
        if (v == "")
        {
            return "1";
        }

        var c = v[v.Length - 1];
        if (c == '0')
        {
            return v[0..(v.Length - 1)] + 1;
        }
        else if (c == '1')
        {
            return v[0..(v.Length - 1)] + 2;
        }
        else if (c == '2')
        {
            return Increment(v[0..(v.Length - 1)]) + '=';
        }
        else if (c == '=')
        {
            return v[0..(v.Length - 1)] + "-";
        }
     
        return v[0..(v.Length - 1)] + 0;
    }
}
