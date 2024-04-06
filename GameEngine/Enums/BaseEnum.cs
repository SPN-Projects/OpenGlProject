namespace GameEngine.Enums;
public class BaseEnum<T>(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
