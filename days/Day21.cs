
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static AoC2022.days.Day16;

namespace AoC2022.days;
internal class Day21
{
    record Monkey(string Name, long? Value, string? MonkeyDep1, string? MonkeyDep2, string? Operand, string? HumnValue);
    public void Run(IEnumerable<string> lines)
    {
        var monkeys = lines.Select(l =>
            {
                var s = l.Split(": ");
                var monkey = s[0];
                var s2 = s[1].Split(" ");
                if (s2.Length == 1)
                {
                    return new Monkey(monkey, long.Parse(s2[0]), null, null, null, null);
                }

                return new Monkey(monkey, null, s2[0], s2[2], s2[1], null);
            })
            .ToDictionary(m => m.Name);

        bool changed = true;
        while(changed)
        {
            changed = false;
            monkeys = monkeys.Values.Select(m =>
            {
                if (m.Value is not null)
                {
                    return m;
                }

                long? v1 = null;
                if (long.TryParse(m.MonkeyDep1!, out var r1))
                {
                    v1 = r1;
                }
                else
                {
                    if (m.MonkeyDep1 != "humn" && monkeys[m.MonkeyDep1!].Value is not null) 
                    {
                        changed = true;
                        m = m with { MonkeyDep1 = monkeys[m.MonkeyDep1!].Value?.ToString() };
                    }
                }

                long? v2 = null;
                if (long.TryParse(m.MonkeyDep2!, out var r2))
                {
                    v2 = r2;
                }
                else
                {
                    if (m.MonkeyDep2 != "humn" && monkeys[m.MonkeyDep2!].Value is not null)
                    {
                        changed = true;
                        m = m with { MonkeyDep2 = monkeys[m.MonkeyDep2!].Value?.ToString() };
                    }
                }

                if (v1 is not null && v2 is not null)
                {
                    changed = true;
                    m = m with
                    {
                        Value = m.Operand switch
                        {
                            "*" => v1 * v2,
                            "/" => v1 / v2,
                            "-" => v1 - v2,
                            "+" => v1 + v2
                        }
                    };
                }

                return m;
            })
            .ToDictionary(m => m.Name);
        }

        //foreach(var monkey in monkeys.Values)
        //{
        //    var str = monkey.Name + ": ";
        //    if( monkey.Value is not null)
        //    {
        //        str += monkey.Value;
        //    }
        //    else
        //    {
        //        str += monkey.MonkeyDep1 + " " + monkey.Operand + " " + monkey.MonkeyDep2;
        //    }
        //    Console.WriteLine(str);
        //}

        var root = monkeys["root"];
        var eq = long.Parse(root.MonkeyDep2!);
        var monkey = monkeys[root.MonkeyDep1!];
        while (true)
        {
            if (monkey.Name == "humn")
            {
                break;
            }

            long? v1 = null;
            if (long.TryParse(monkey.MonkeyDep1!, out var r1))
            {
                v1 = r1;
            }

            long? v2 = null;
            if (long.TryParse(monkey.MonkeyDep2!, out var r2))
            {
                v2 = r2;
            }
            
            if (v1 is not null)
            {
                eq = monkey.Operand switch
                {
                    "*" => eq / v1.Value, // 9 * x = 15 => x = 15 / 9
                    "/" => v1.Value / eq, // 9 / x = 15 => x = 9 / 15
                    "-" => v1.Value - eq, // 9 - x = 15 => x = 9 - 15
                    "+" => eq - v1.Value // 9 + x = 15 => x = 15 - 9
                };
                monkey = monkeys[monkey.MonkeyDep2!];
            }
            else
            {
                eq = monkey.Operand switch
                {
                    "*" => eq / v2.Value, // x * 9 = 15 => x = 15 / 9
                    "/" => eq * v2.Value, // x / 9 = 15 => x = 15 * 9
                    "-" => eq + v2.Value, // x - 9 = 15 => x = 15 + 9
                    "+" => eq - v2.Value // x + 9 = 15 => x = 15 - 9
                };
                monkey = monkeys[monkey.MonkeyDep1!];
            }
        }

        Console.WriteLine(eq);
        var m = root.MonkeyDep1;

        //var s = Calculate(monkeys, monkeys[m]);
        //while (m != "humn")
        //{
        //    var monkey1 = monkeys[m];
        //    var monkey2 = monkeys[monkey1.MonkeyDep1];
        //    var monkey3 = monkeys[monkey1.MonkeyDep2];

        //}

        while (monkeys.Values.Any(m => m.Value is null))
        {
            monkeys = monkeys.Values
                .Select(m =>
                {
                    if (m.Value is not null)
                    {
                        return m;
                    }

                    if (!monkeys.TryGetValue(m.MonkeyDep1!, out var m1))
                    {
                        return m;
                    }
                    if (!monkeys.TryGetValue(m.MonkeyDep2!, out var m2))
                    {
                        return m;
                    }

                    var value = m.Operand switch
                    {
                        "*" => m1.Value * m2.Value,
                        "/" => m1.Value / m2.Value,
                        "-" => m1.Value - m2.Value,
                        "+" => m1.Value + m2.Value
                    };

                    return m with { Value = value };
                })
                .ToDictionary(m => m.Name);
        }


        var equation = monkeys["root"].HumnValue!;

        //var sides = equation.Split("=");
        //var side1 = Reduce(sides[0]);
        //var side2 = Reduce(sides[1]);


    }


    //record Monkey(string Name, long? Value, string? MonkeyDep1, string? MonkeyDep2, string? Operand, string? HumnValue);
    //public void Run(IEnumerable<string> lines)
    //{
    //    var monkeys = lines.Select(l =>
    //        {
    //            var s = l.Split(": ");
    //            var monkey = s[0];
    //            var s2 = s[1].Split(" ");
    //            if (s2.Length == 1)
    //            {
    //                return new Monkey(monkey, long.Parse(s2[0]), null, null, null, null);
    //            }

    //            return new Monkey(monkey, null, s2[0], s2[2], s2[1], null);
    //        })
    //        .ToDictionary(m => m.Name);

    //    while (monkeys.Values.Any(m => m.Value is null && m.HumnValue is null))
    //    {
    //        monkeys = monkeys.Values
    //            .Select(m =>
    //            {
    //                if (m.Value is not null || m.HumnValue is not null)
    //                {
    //                    return m;
    //                }

    //                var firstIsHumn = false;
    //                var secondIsHumn = false;

    //                if (!monkeys.TryGetValue(m.MonkeyDep1!, out var m1) || (m1.HumnValue is null && m1.Value is null))
    //                {
    //                    return m;
    //                }

    //                if (!monkeys.TryGetValue(m.MonkeyDep2!, out var m2) || (m2.HumnValue is null && m2.Value is null))
    //                {
    //                    return m;
    //                }

    //                if (m.Name == "root")
    //                {
    //                    return m with { HumnValue = (m1!.Value?.ToString() ?? m1.HumnValue!) + "=" + (m2!.Value?.ToString() ?? m2.HumnValue!) };
    //                }

    //                string? firstHumn = null;
    //                long? firstValue = null;
    //                if (firstIsHumn && m.MonkeyDep1! == "humn")
    //                {
    //                    firstHumn = "x";
    //                }
    //                else if (m1!.HumnValue is not null)
    //                {
    //                    firstHumn = $"({m1!.HumnValue!})";
    //                }
    //                else
    //                {
    //                    firstValue = m1!.Value!;
    //                }

    //                string? secondHumn = null;
    //                long? secondValue = null;
    //                if (firstIsHumn && m.MonkeyDep2! == "humn")
    //                {
    //                    secondHumn = "x";
    //                }
    //                else if (m2!.HumnValue is not null)
    //                {
    //                    secondHumn = $"({m2!.HumnValue!})";
    //                }
    //                else
    //                {
    //                    secondValue = m2!.Value;
    //                }

    //                string? humnValue = null;
    //                long? value = null;
    //                if (firstHumn is not null && secondHumn is not null)
    //                {
    //                    humnValue = $"({firstHumn}{m.Operand}{secondHumn})";
    //                }
    //                else if (firstHumn is not null && secondValue is not null)
    //                {
    //                    humnValue = m.Operand switch
    //                    {
    //                        "*" => m1.Value * m2.Value,
    //                        "/" => m1.Value / m2.Value,
    //                        "-" => m1.Value - m2.Value,
    //                        "+" => m1.Value + m2.Value
    //                    };
    //                }
    //                else if (firstValue is not null && secondHumn is not null)
    //                {

    //                }
    //                else
    //                {
    //                    value = m.Operand switch
    //                    {
    //                        "*" => m1.Value * m2.Value,
    //                        "/" => m1.Value / m2.Value,
    //                        "-" => m1.Value - m2.Value,
    //                        "+" => m1.Value + m2.Value
    //                    };
    //                }

    //                return m with { Value = value, HumnValue = humnValue };

    //            })
    //            .ToDictionary(m => m.Name);
    //    }


    //    var equation = monkeys["root"].HumnValue!;

    //    var sides = equation.Split("=");


    //}

}
