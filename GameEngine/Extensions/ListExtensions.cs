namespace GameEngine.Extensions;
public static class ListExtensions
{
    public static IEnumerable<T> TakeRandom<T>(this List<T> list, int count)
    {
        var random = new Random();
        var taken = new List<T>();

        for (var i = 0; i < count; i++)
        {
            var index = random.Next(0, list.Count);
            taken.Add(list[index]);
        }

        return taken;
    }

    public static IEnumerable<T> TakeRandom<T>(this List<T> list, int count, Random random)
    {
        var taken = new List<T>();

        for (var i = 0; i < count; i++)
        {
            var index = random.Next(0, list.Count);
            taken.Add(list[index]);
        }

        return taken;
    }

    public static IEnumerable<(int index, T quad)> TakeRandomWithIndex<T>(this List<T> list, int count)
    {
        var random = new Random();
        var taken = new List<(int, T)>();

        for (var i = 0; i < count; i++)
        {
            var index = random.Next(0, list.Count);
            taken.Add((index, list[index]));
        }

        return taken;
    }

    public static void AddRange<T>(this List<T> list, params T[] items)
        => list.AddRange(items);
}
