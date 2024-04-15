namespace SPPA.Logic.Extensions;

public static class DictionaryExtension
{
    public static Dictionary<TKey, TValue> MergeInPlace<TKey, TValue>(this Dictionary<TKey, TValue> left, Dictionary<TKey, TValue> right)
    where TKey : notnull
    where TValue : notnull
    {
        foreach (var rItem in right)
        {
            if (!left.ContainsKey(rItem.Key))
            {
                left.Add(rItem.Key, rItem.Value);
            }
        }

        return left;
    }
}
