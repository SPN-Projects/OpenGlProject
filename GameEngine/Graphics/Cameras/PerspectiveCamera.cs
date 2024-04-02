using GameEngine.Logging;
using GameEngine.Math;
using OpenTK.Mathematics;

namespace GameEngine.Graphics.Cameras;
public class PerspectiveCamera : Camera
{
    private readonly float _minFieldOfView;
    private readonly float _maxFieldOfView;

    private float _fieldOfView;
    public float FieldOfView
    {
        get => _fieldOfView;
        set
        {
            var newValue = SPNMath.Clamp(value, _minFieldOfView, _maxFieldOfView);

            if (_fieldOfView == newValue)
            {
                return;
            }

            _fieldOfView = newValue;


            Logger.EngineLogger.Debug($"Changed Field of View: {_fieldOfView}");
            UpdateProjectionMatrix();
        }
    }

    public float AspectRatio { get; set; }
    public float NearPlane { get; private set; }
    public float FarPlane { get; private set; }

    public PerspectiveCamera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane, float minFieldOfView = EngineConstants.MinFieldOfView, float maxFieldOfView = EngineConstants.MaxFieldOfView) : base()
    {
        AspectRatio = aspectRatio;
        NearPlane = nearPlane;
        FarPlane = farPlane;

        _minFieldOfView = minFieldOfView;
        _maxFieldOfView = maxFieldOfView;

        FieldOfView = fieldOfView;
    }

    public override void UpdateViewProjectionMatrix()
    {
        View = Matrix4.CreateTranslation(-Position);
        ViewProjectionMatrix = View * Projection;
    }

    private void UpdateProjectionMatrix()
    {
        Projection = Matrix4.CreatePerspectiveFieldOfView(SPNMath.ToRadians(_fieldOfView), AspectRatio, NearPlane, FarPlane);
        UpdateViewProjectionMatrix();
    }
}
