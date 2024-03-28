namespace GameEngine.Extensions;
public static class Vector2Extenstions
{
    public static OpenTK.Mathematics.Vector2 ToOpenTK(this System.Numerics.Vector2 vector)
        => new(vector.X, vector.Y);

    public static OpenTK.Mathematics.Vector2i ToOpenTKi(this System.Numerics.Vector2 vector)
        => new((int)vector.X, (int)vector.Y);

    public static System.Numerics.Vector2 ToNumerics(this OpenTK.Mathematics.Vector2 vector)
        => new(vector.X, vector.Y);

    public static System.Numerics.Vector2 ToNumerics(this OpenTK.Mathematics.Vector2i vector)
        => new(vector.X, vector.Y);
}
