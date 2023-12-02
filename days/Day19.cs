
using System.Diagnostics;
using System.Text.RegularExpressions;
using static AoC2022.days.Day16;

namespace AoC2022.days;
internal class Day19
{
    record StashGenerators(long Ore = 0, long Clay = 0, long Obsidian = 0, long Geode = 0);
    enum Robot { Ore, Clay, Obsidian, Geode }
    record Stash(long Ore = 0, long Clay = 0, long Obsidian = 0, long Geode = 0);
    record Cost(long Ore = 0, long Clay = 0, long Obsidian = 0, long Geode = 0);
    record Blueprint(long OreOre, long ClayOre, long ObsidianOre, long ObsidianClay, long GeodeOre, long GeodeObsidian);
    public void Run(IEnumerable<string> lines)
    {
        var watch = Stopwatch.StartNew();

        var blueprints = lines
            .Select(l =>
            {
                var oreM = Regex.Match(l, "Each ore robot costs (\\d+) ore.");
                var clayM = Regex.Match(l, "Each clay robot costs (\\d+) ore.");
                var obsidianM = Regex.Match(l, "Each obsidian robot costs (\\d+) ore and (\\d+) clay.");
                var geodeM = Regex.Match(l, "Each geode robot costs (\\d+) ore and (\\d+) obsidian.");


                return new Blueprint(
                    long.Parse(oreM.Groups[1].Value),
                    long.Parse(clayM.Groups[1].Value),
                    long.Parse(obsidianM.Groups[1].Value),
                    long.Parse(obsidianM.Groups[2].Value),
                    long.Parse(geodeM.Groups[1].Value),
                    long.Parse(geodeM.Groups[2].Value)
                );
            });

        Console.WriteLine(watch.Elapsed);
        watch.Restart();

        var r1 = blueprints.Select((b, idx) => (idx + 1) * MaxGeode(b, 24)).Sum();
        Console.WriteLine(watch.Elapsed);
        Console.WriteLine(r1);
        watch.Restart();

        var r = blueprints.Take(3).Select((b, idx) => MaxGeode(b, 32)).ToList();

        Console.WriteLine(watch.Elapsed);
        Console.WriteLine(r[0] * r[1] * r[2]);
    }

    record QueueItem(long Minutes, Stash Stash, Stash Generators);

    private long MaxGeode(Blueprint blueprint, long maxMinutes)
    {
        var visited = new HashSet<QueueItem>();
        var visited2 = new Dictionary<long, Stash>();
        long maximumGeode = 0;
        var queue = new LinkedList<QueueItem>();
        queue.AddFirst(new QueueItem(0, new(), new Stash(Ore: 1)));
        var maxOre = new[] { blueprint.OreOre, blueprint.GeodeOre, blueprint.ClayOre, blueprint.ObsidianOre }.Max();
        while (queue.First is not null)
        {
            var item = queue.First.Value;
            queue.RemoveFirst();

            List<Robot> choices = new List<Robot>();

            var timeLeft = maxMinutes - item.Minutes;

            if (item.Stash.Geode + item.Generators.Geode * timeLeft + ((timeLeft * timeLeft) / 2) < maximumGeode)
            {
                continue;
            }

            if (item.Stash.Ore + (timeLeft * item.Generators.Ore) < maxOre * timeLeft)
            {
                choices.Add(Robot.Ore);
            }

            if (item.Stash.Ore + (timeLeft * item.Generators.Clay) < blueprint.ObsidianClay * timeLeft)
            {
                choices.Add(Robot.Clay);
            }

            if (item.Stash.Ore + (timeLeft * item.Generators.Obsidian) < blueprint.GeodeObsidian * timeLeft
                && item.Generators.Clay != 0)
            {
                choices.Add(Robot.Obsidian);
            }

            if (item.Generators.Obsidian != 0)
            {
                choices.Add(Robot.Geode);
            }

            foreach (var robot in choices)
            {
  
                var nItem = Build(blueprint, robot, item.Stash, item.Generators);

                nItem.Minutes += item.Minutes;
                if (nItem.Minutes >= maxMinutes)
                {
                    var endGeode = item.Stash.Geode + (item.Generators.Geode * timeLeft);
                    if (endGeode > maximumGeode)
                    {
                        maximumGeode = endGeode;
                        //Console.WriteLine("new max: " + maximumGeode);
                    }
                    continue;
                }


                queue.AddFirst(new QueueItem(nItem.Minutes, nItem.Stash, nItem.Generators));
            }
        }

        return maximumGeode;
    }

    private (long Minutes, Stash Stash, Stash Generators) Build(Blueprint blueprint, Robot robot, Stash stash, Stash generators)
    {
        if (robot == Robot.Ore)
        {
            var oreMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.OreOre - stash.Ore) / (double)generators.Ore);

            var minutes = oreMinutes + 1;

            var iStash = IncrementStash(minutes, stash, generators);
            return (
                minutes,
                iStash with
                {
                    Ore = iStash.Ore - blueprint.OreOre
                },
                generators with { Ore = generators.Ore + 1}
            );
        }
        else if (robot == Robot.Clay)
        {
            var oreMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.ClayOre - stash.Ore) / (double)generators.Ore);

            var minutes = oreMinutes + 1;

            var iStash = IncrementStash(minutes, stash, generators);
            return (
                  minutes,
                  iStash with
                  {
                      Ore = iStash.Ore - blueprint.ClayOre
                  },
                  generators with { Clay = generators.Clay + 1 }
              );
        }
        else if (robot == Robot.Obsidian)
        {
            var oreMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.ObsidianOre - stash.Ore) / (double)generators.Ore);
            var clayMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.ObsidianClay - stash.Clay) / (double)generators.Clay);

            var minutes = Math.Max(oreMinutes, clayMinutes) + 1;

            var iStash = IncrementStash(minutes, stash, generators);

            return (
                  minutes,
                  iStash with
                  {
                      Ore = iStash.Ore - blueprint.ObsidianOre,
                      Clay = iStash.Clay - blueprint.ObsidianClay,
                  },
                  generators with { Obsidian = generators.Obsidian + 1 }
              );
        }
        else
        {
            var oreMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.GeodeOre - stash.Ore) / (double)generators.Ore);
            var obsidianMinutes = (long)Math.Ceiling(Math.Max(0, blueprint.GeodeObsidian - stash.Obsidian) / (double)generators.Obsidian);

            var minutes = Math.Max(oreMinutes, obsidianMinutes) + 1;

            var iStash = IncrementStash(minutes, stash, generators);

            return (
                  minutes,
                  iStash with
                  {
                      Ore = iStash.Ore - blueprint.GeodeOre,
                      Obsidian = iStash.Obsidian - blueprint.GeodeObsidian,
                  },
                  generators with { Geode = generators.Geode + 1 }
              );
        }
    }

    private Stash IncrementStash(long minutes, Stash stash, Stash generators)
    {
        return new Stash(
            stash.Ore + (generators.Ore * minutes),
            stash.Clay + (generators.Clay * minutes),
            stash.Obsidian + (generators.Obsidian * minutes),
            stash.Geode + (generators.Geode * minutes)
        );
    }
}
