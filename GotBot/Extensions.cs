namespace GotBot;

static class EnumerableExtension
{
#warning Может быть бесконечный цикл
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        var random = new Random();
        for (var i = list.Count; i > 0; i--)
        {
            int randIndex = random.Next(i);
            (list[0], list[randIndex]) = (list[randIndex], list[0]);
        }
        return list;
    }
}