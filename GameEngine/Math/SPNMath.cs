namespace GameEngine.Math;
public static class SPNMath
{
    public static float ToRadians(float degrees)
        => degrees * (MathF.PI / 180.0f);

    public static float Clamp(float value, float min, float max)
        => MathF.Max(min, MathF.Min(max, value));
}
