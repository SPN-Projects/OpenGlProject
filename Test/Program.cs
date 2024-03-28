namespace Test;

public class Program
{
    public static TestGame? Game { get; set; }

    private static void Main()
        => Game = new TestGame(GameConstants.Title);
}
