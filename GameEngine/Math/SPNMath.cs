using OpenTK.Mathematics;

namespace GameEngine.Math;
public static class SPNMath
{
    public static float ToRadians(float degrees)
        => degrees * (MathF.PI / 180.0f);

    public static float Clamp(float value, float min, float max)
        => MathF.Max(min, MathF.Min(max, value));

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        => a + ((b - a) * t);

    public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        => a + ((b - a) * t);

    public static double SquareDistance(Vector3 position, Vector3 cubePosition)
        => MathF.Pow(position.X - cubePosition.X, 2) + MathF.Pow(position.Y - cubePosition.Y, 2) + MathF.Pow(position.Z - cubePosition.Z, 2);
}