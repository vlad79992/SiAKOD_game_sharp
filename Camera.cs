using System;
using System.Collections.Generic;
using System.Text;

internal class Camera
{
    private (double, double) position;
    private float aspectRatio;
    private float scale;
    public Camera(
            (double, double) position = default((double, double)),
            float aspectRatio = 1.0f,
            float scale = 1.0f
        )
    {
        this.position = position;
        this.aspectRatio = aspectRatio;
        this.scale = scale;
    }
    public (double, double) Position 
    { 
        get => position;
        set
        {
            bool cameraChanged = false;
            if (position != value)
                cameraChanged = true;
            position = value;
            if (cameraChanged)
                CameraChanged.Invoke();
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
                CameraChanged.Invoke();
        }
    }
    //длина самой длинной стороны окна
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
                CameraChanged.Invoke();
        }
    }
    public IEnumerable<(int x, int y)> VisiblePoints
    {
        get
        {
            float width, height;
            if (AspectRatio >= 1)
            {
                width = Scale;
                height = Scale / AspectRatio;
            }
            else
            {
                height = Scale;
                width = Scale * AspectRatio;
            }

            double left = Position.Item1;
            double right = Position.Item1 + width;
            double bottom = Position.Item2;
            double top = Position.Item2 + height;

            int xMin = (int)Math.Ceiling(left);
            int xMax = (int)Math.Floor(right);
            int yMin = (int)Math.Ceiling(bottom);
            int yMax = (int)Math.Floor(top);

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
    public Action CameraChanged;
}