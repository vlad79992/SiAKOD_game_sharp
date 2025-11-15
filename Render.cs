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
        float pointSizeX = (camera.Scale / 1) * ((camera.AspectRatio < 1) ? (camera.AspectRatio) : (1));
        float pointSizeY = (camera.Scale / 1) * ((camera.AspectRatio > 1) ? (camera.AspectRatio) : (1));

        var gridPositions = camera.VisiblePoints.ToArray();
        var color = SFML.Graphics.Color.White;

        va.PrimitiveType = PrimitiveType.Triangles;
        va.Resize((uint)(gridPositions.Length * 6)); // * 6 потому что каждый квадрат состоит из двух треугольников,
                                                     // в каждом треугольнике есть три вершины
        for (uint i = 0; i < gridPositions.Length; i++)
        {
            float posX = (gridPositions[i].x * camera.Scale * 2 + camera.Scale / 2) / ((camera.AspectRatio < 1) ? (camera.AspectRatio) : (1));
            float posY = (gridPositions[i].y * camera.Scale * 2 + camera.Scale / 2) * ((camera.AspectRatio > 1) ? (camera.AspectRatio) : (1));



            va[i * 6 + 0] = new Vertex(new Vector2f(posX, posY), color);
            va[i * 6 + 1] = new Vertex(new Vector2f(posX + pointSizeX, posY), color);
            va[i * 6 + 2] = new Vertex(new Vector2f(posX + pointSizeX, posY + pointSizeY), color);

            va[i * 6 + 3] = new Vertex(new Vector2f(posX, posY), color);
            va[i * 6 + 4] = new Vertex(new Vector2f(posX + pointSizeX, posY + pointSizeY), color);
            va[i * 6 + 5] = new Vertex(new Vector2f(posX, posY + pointSizeY), color);
        }
    }
}
