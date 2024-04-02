using OpenTK.Mathematics;

namespace GameEngine.Graphics.Cameras;
public class IdentityCamera : Camera
{
    public IdentityCamera() : base()
    {
    }

    public override void UpdateViewProjectionMatrix()
    {
        View = Matrix4.Identity;
        ViewProjectionMatrix = View * Projection;
    }
}
