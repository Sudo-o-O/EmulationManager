namespace EmulationManager.Switch;

public sealed class SwitchKeySet
{
    public Dictionary<string, byte[]> Keys { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public int Count => Keys.Count;

    public bool Contains(string name)
    {
        return Keys.ContainsKey(name);
    }

    public bool TryGet(
        string name,
        out byte[]? value)
    {
        return Keys.TryGetValue(name, out value);
    }

    public byte[] GetRequired(string name)
    {
        if (!Keys.TryGetValue(name, out byte[]? value))
        {
            throw new KeyNotFoundException(
                $"The required key '{name}' was not found.");
        }

        return value;
    }
}