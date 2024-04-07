namespace GameEngine.Utils;
public class ResourceManager<T>
{
    private static readonly Lazy<ResourceManager<T>> _instance = new(()
        => new ResourceManager<T>());
    public static ResourceManager<T> Instance => _instance.Value;

    private readonly Dictionary<string, T> _resources = [];

    public T? GetResource(string name)
        => _resources.TryGetValue(name, out var value) ? value : default;

    public T AddResource(string name, T resource)
        => _resources.TryAdd(name, resource) ? resource : GetResource(name)!;

    public void RemoveResource(string name)
    {
        if (!_resources.ContainsKey(name))
        {
            return;
        }

        _ = _resources.Remove(name);
    }

    public void ClearResources()
        => _resources.Clear();
}
