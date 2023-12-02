namespace AoC2022.days
{
    public record WeightedPath(HashSet<PathPoint> Path, long Weight);
    public record PathPoint(long X, long Y, long Z);
    public class PathFinding
    {
        public static WeightedPath? FindPath(
            PathPoint from,
            Func<PathPoint, bool> isEnd,
            Func<PathPoint, int> getWeight,
            Func<PathPoint, PathPoint, bool> isBlocked,
            bool allowDiag = false,
            bool is3d = false
        )
        {
            WeightedPath? cheapestEnd = null;
            var cheapest = new Dictionary<PathPoint, WeightedPath>();
            var queue = new LinkedList<(HashSet<PathPoint>, long, PathPoint)>();
            queue.AddLast((new HashSet<PathPoint>(), -getWeight(from), from));
            while(queue.First is not null)
            {
                var (prevPath, prevWeight, newNode) = queue.First.Value;
                queue.RemoveFirst();

                var weight = prevWeight + getWeight(newNode);

                if (cheapest.TryGetValue(newNode, out var value) && weight >= value.Weight)
                {
                    continue;
                }

                if (cheapestEnd is not null && weight >= cheapestEnd.Weight)
                {
                    continue;
                }

                var newPath = new HashSet<PathPoint>(prevPath)
                {
                    newNode
                };
                var wp = new WeightedPath(newPath, weight);
                
                cheapest[newNode] = wp;
   
                if (isEnd(newNode))
                {
                    return wp;
                }

                var neighbours = Neighbours(newNode, allowDiag, is3d)
                    .Where(n => !isBlocked(newNode, n))
                    .Where(n => !prevPath.Contains(n))
                    .Select(n => (newPath, weight, n));

                foreach(var neighbour in neighbours)
                {
                    queue.AddLast(neighbour);
                }
            }

            return cheapestEnd;
        }

        public static IEnumerable<PathPoint> Neighbours(PathPoint point, bool allowDiag, bool is3d)
        {
            yield return new PathPoint(point.X, point.Y - 1, point.Z); // N
            yield return new PathPoint(point.X, point.Y + 1, point.Z); // S
            yield return new PathPoint(point.X - 1, point.Y, point.Z); // W
            yield return new PathPoint(point.X + 1, point.Y, point.Z); // E

            if (allowDiag)
            {
                yield return new PathPoint(point.X - 1, point.Y - 1, point.Z); // NW
                yield return new PathPoint(point.X - 1, point.Y + 1, point.Z); // SW
                yield return new PathPoint(point.X + 1, point.Y - 1, point.Z); // NE
                yield return new PathPoint(point.X + 1, point.Y + 1, point.Z); // SE
            }

            if (is3d)
            {
                yield return new PathPoint(point.X, point.Y, point.Z - 1);
                yield return new PathPoint(point.X, point.Y, point.Z + 1);

                if (allowDiag)
                {
                    yield return new PathPoint(point.X - 1, point.Y - 1, point.Z - 1);
                    yield return new PathPoint(point.X - 1, point.Y + 1, point.Z - 1);
                    yield return new PathPoint(point.X + 1, point.Y - 1, point.Z - 1);
                    yield return new PathPoint(point.X + 1, point.Y + 1, point.Z - 1);

                    yield return new PathPoint(point.X - 1, point.Y - 1, point.Z + 1);
                    yield return new PathPoint(point.X - 1, point.Y + 1, point.Z + 1);
                    yield return new PathPoint(point.X + 1, point.Y - 1, point.Z + 1);
                    yield return new PathPoint(point.X + 1, point.Y + 1, point.Z + 1);

                    yield return new PathPoint(point.X, point.Y - 1, point.Z - 1);
                    yield return new PathPoint(point.X, point.Y + 1, point.Z - 1);
                    yield return new PathPoint(point.X, point.Y - 1, point.Z + 1);
                    yield return new PathPoint(point.X, point.Y + 1, point.Z + 1);

                    yield return new PathPoint(point.X - 1, point.Y, point.Z - 1);
                    yield return new PathPoint(point.X + 1, point.Y, point.Z - 1);
                    yield return new PathPoint(point.X - 1, point.Y, point.Z + 1);
                    yield return new PathPoint(point.X + 1, point.Y, point.Z + 1);
                }
            }
        }
    }
}
