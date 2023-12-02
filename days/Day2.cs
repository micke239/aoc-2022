using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day2
{
    public void Run(IEnumerable<string> lines)
    {
        var h = lines.Select(l =>
        {
            var split = l.Split(" ");
            var opponent = split[0];
            var mine = split[1];

            var outcome = opponent switch
            {
                "A" => mine switch
                {
                    "X" => 3,
                    "Y" => 1,
                    "Z" => 2
                },
                "B" => mine switch
                {
                    "X" => 1,
                    "Y" => 2,
                    "Z" => 3
                }
                ,
                "C" => mine switch
                {
                    "X" => 2,
                    "Y" => 3,
                    "Z" => 1
                },
            };

            var bonus = mine switch
            {
                "X" => 0,
                "Y" => 3,
                "Z" => 6
            };

            return outcome + bonus;
        }).Sum();

        Console.WriteLine(h);
    }
}
