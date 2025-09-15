namespace SetlistManager.App.Services;

public static class ShuffleService
{
    private static readonly Random rng = new();

    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}