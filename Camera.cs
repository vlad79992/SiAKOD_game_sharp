internal class Camera(
        (double, double) position = default((double, double)),
        float aspectRatio = 1.0f,
        float scale = 1.0f
        )
{
    private (double x, double y) position = position;
    private float aspectRatio = aspectRatio;
    private float scale = scale;
    public Action? CameraChanged;

    public (double X, double Y) Position 
    { 
        get => position;
        set
        {
            bool cameraChanged = false;
            if (position != value)
                cameraChanged = true;
            position = value;
            if (cameraChanged)
                CameraChanged?.Invoke();
        }
    }
    public float AspectRatio 
    { 
        get => aspectRatio;
        set
        {
            bool cameraChanged = false;
            if (aspectRatio != value)
                cameraChanged = true;
            aspectRatio = value;
            if (cameraChanged)
                CameraChanged?.Invoke();
        }
    }
    public float Scale 
    { 
        get => scale;
        set
        {
            bool cameraChanged = false;
            if (scale != value)
                cameraChanged = true;
            scale = value;
            if (cameraChanged)
                CameraChanged?.Invoke();
        }
    }
    public IEnumerable<(int X, int Y)> VisiblePoints
    {
        get
        {
            int pointsAlongLongerSide = (int)Math.Ceiling(Scale);
            int pointsAlongShorterSide = (int)Math.Ceiling(Scale / aspectRatio);

            int centerX = (int)Math.Round(position.Item1);
            int centerY = (int)Math.Round(position.Item2);

            int halfX = pointsAlongLongerSide / 2;
            int halfY = pointsAlongShorterSide / 2;

            int xMin = centerX - halfX;
            int xMax = centerX + halfX;
            int yMin = centerY - halfY;
            int yMax = centerY + halfY;

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    yield return (x, y);
                }
            }
        }
    }
    public (double width, double height) GetVisibleArea()
    {
        if (AspectRatio >= 1)
            return (Scale, Scale / AspectRatio);
        return (Scale * AspectRatio, Scale);
    }
    public (double X, double Y) ScreenToWorld(int screenX, int screenY, uint screenWidth, uint screenHeight)
    {
        var (worldWidth, worldHeight) = GetVisibleArea();

        double worldLeft = position.x - worldWidth / 2.0;
        double worldTop = position.y - worldHeight / 2.0;

        double normalizedX = (double)screenX / screenWidth;
        double normalizedY = (double)screenY / screenHeight;

        double worldX = worldLeft + normalizedX * worldWidth;
        double worldY = worldTop + normalizedY * worldHeight;

        return (worldX, worldY);
    }
    public (int X, int Y) WorldToScreen(double worldX, double worldY, uint screenWidth, uint screenHeight)
    {
        var (worldWidth, worldHeight) = GetVisibleArea();

        double worldLeft = position.x - worldWidth / 2.0;
        double worldTop = position.y - worldHeight / 2.0;

        double normalizedX = (worldX - worldLeft) / worldWidth;
        double normalizedY = (worldY - worldTop) / worldHeight;

        int screenX = (int)Math.Round(normalizedX * screenWidth);
        int screenY = (int)Math.Round(normalizedY * screenHeight);

        return (screenX, screenY);
    }
}