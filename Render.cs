using SFML.Graphics;
using SFML.Graphics.Glsl;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

internal class Render
{
    private SFML.Graphics.RenderWindow window;
    private Camera camera;
    private SFML.Graphics.VertexArray va = new();
    public Render(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        // обновляем вершины только при изменении камеры, чтобы не заниматься этим каждый кадр
        camera.CameraChanged += () => UpdateVA();
    }

    public void DrawGrid()
    {
        window.Draw(va);
    }
    private void UpdateVA()
    {
        uint windowWidth = window.Size.X;
        uint windowHeight = window.Size.Y;

        var visibleArea = camera.GetVisibleArea();
        float visibleWidth = (float)visibleArea.width;
        float visibleHeight = (float)visibleArea.height;

        float worldScale = Math.Min(
            (float)windowWidth / visibleWidth,
            (float)windowHeight / visibleHeight
        );

        float scaleX = worldScale;
        float scaleY = worldScale;

        float pointSizeInWorld = 0.1f; // Размер квадрата
        float pointSizeX = pointSizeInWorld * worldScale;
        float pointSizeY = pointSizeInWorld * worldScale;

        var gridPositions = camera.VisiblePoints.ToArray();
        var color = SFML.Graphics.Color.White;

        //Console.WriteLine($"{windowWidth} {windowHeight}");
        //Console.WriteLine($"{scaleX} {scaleY}");
        //Console.WriteLine($"{gridPositions.Length}");

        va.PrimitiveType = PrimitiveType.Triangles;
        va.Resize((uint)(gridPositions.Length * 6));

        for (uint i = 0; i < gridPositions.Length; i++)
        {
            float worldX = gridPositions[i].x;
            float worldY = gridPositions[i].y;

            float screenX = (worldX - (float)camera.Position.Item1) * scaleX + windowWidth / 2f;
            float screenY = ((float)camera.Position.Item2 - worldY) * scaleY + windowHeight / 2f;

            float halfSizeX = pointSizeX / 2f;
            float halfSizeY = pointSizeY / 2f;

            float left = screenX - halfSizeX;
            float right = screenX + halfSizeX;

            float top = screenY - halfSizeY;
            float bottom = screenY + halfSizeY;

            //Console.WriteLine($"{left} {right} {top} {bottom}");

            // Первый треугольник
            va[i * 6 + 0] = new Vertex(new Vector2f(left, bottom), color);
            va[i * 6 + 1] = new Vertex(new Vector2f(right, bottom), color);
            va[i * 6 + 2] = new Vertex(new Vector2f(right, top), color);
            // Второй треугольник
            va[i * 6 + 3] = new Vertex(new Vector2f(left, bottom), color);
            va[i * 6 + 4] = new Vertex(new Vector2f(right, top), color);
            va[i * 6 + 5] = new Vertex(new Vector2f(left, top), color);
        }
    }
}
