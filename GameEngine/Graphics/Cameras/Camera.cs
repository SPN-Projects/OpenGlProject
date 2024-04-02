using OpenTK.Mathematics;

namespace GameEngine.Graphics.Cameras;
public abstract class Camera
{
    private Vector3 _position;
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            UpdateViewProjectionMatrix();
        }
    }

    protected Matrix4 View { get; set; }
    protected Matrix4 Projection { get; set; }

    public Matrix4 ViewProjectionMatrix { get; protected set; }

    public Camera()
    {
        View = Matrix4.Identity;
        Projection = Matrix4.Identity;
        ViewProjectionMatrix = Matrix4.Identity;

        Position = Vector3.Zero;
    }

    public abstract void UpdateViewProjectionMatrix();
}
