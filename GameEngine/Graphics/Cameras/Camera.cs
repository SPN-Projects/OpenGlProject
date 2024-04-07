using OpenTK.Mathematics;

namespace GameEngine.Graphics.Cameras;
public abstract class Camera
{
    public Vector3 Position { get; set; }

    public Matrix4 View { get; protected set; }
    public Matrix4 Projection { get; protected set; }

    public Camera()
    {
        View = Matrix4.Identity;
        Projection = Matrix4.Identity;

        Position = Vector3.Zero;
    }
}
