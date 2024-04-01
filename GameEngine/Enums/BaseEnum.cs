namespace GameEngine.Enums;
public class BaseEnum<T>
{
    public int Id { get; }
    public string Name { get; }

    public BaseEnum(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
