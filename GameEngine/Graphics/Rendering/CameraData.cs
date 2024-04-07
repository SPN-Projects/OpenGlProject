using OpenTK.Mathematics;

namespace GameEngine.Graphics.Rendering;
public struct CameraData
{
    public Vector4 CameraPosition { get; internal set; }
    public Matrix4 ProjectionMatrix { get; internal set; }
}
