using OpenTK.Mathematics;

namespace GameEngine.Graphics.Rendering;
public struct CameraData
{
    public Matrix4 ViewProjectionMatrix { get; internal set; }
}
