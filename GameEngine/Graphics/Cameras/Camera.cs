using OpenTK.Mathematics;

namespace GameEngine.Graphics.Cameras;
public class Camera
{
    public Matrix4 ViewProjectionMatrix { get; internal set; }

    public Camera(Matrix4? viewProjectionMatrix = null)
    {
        ViewProjectionMatrix = viewProjectionMatrix ?? Matrix4.Identity;
    }
}
