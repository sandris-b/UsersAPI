using Microsoft.EntityFrameworkCore;

namespace UsersAPI;

public static class Extensions
{
    public static HttpClient Client = new();

    public static string GetAdditionalColumn(this string value)
    {
        var splitted = value.Split(' ');
        if (splitted == null || splitted.Length < 2)
            return string.Empty;

        return $"{splitted[0][0]}{splitted[1]}@ibsat.com";
    }

    public static void AddOrUpdateRange<TEntity>(DbSet<TEntity> set, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            foreach (var entity in entities)
            {
                _ = !set.Any(e => e == entity) ? set.Add(entity) : set.Update(entity);
            }
        }
}