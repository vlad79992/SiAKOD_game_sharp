using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Color = SFML.Graphics.Color;

internal class Render
{
    private readonly RenderWindow window;
    private readonly Camera camera;
    private readonly float pointSizeInWorld = 0.1f;
    private readonly VertexArray gridVA = new(PrimitiveType.Triangles);
    private readonly VertexArray selectionVA = new(PrimitiveType.Triangles, 48);
    private readonly VertexArray linesVA = new(PrimitiveType.Triangles);
    
    private List<(Line line, bool blue)> linesToDraw = new List<(Line, bool)>();

    public Render(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        camera.CameraChanged += () => UpdateGridVA();
        camera.CameraChanged += () => UpdateLinesVA();
    }

    public void DrawGrid()
    {
        window.Draw(gridVA);
    }
    
    public void DrawLines()
    {
        window.Draw(linesVA);
    }
    public void AddLine(Line line, bool blue)
    {
        linesToDraw.Add((line, blue));
        UpdateLinesVA();
    }
    private void UpdateLinesVA()
    {
        var (windowWidth, windowHeight) = window.Size;
        var (visibleWidth, visibleHeight) = camera.GetVisibleArea();

        float worldScale = Math.Min(
            (float)windowWidth / (float)visibleWidth,
            (float)windowHeight / (float)visibleHeight
        );
        
        float pointSize = (pointSizeInWorld * worldScale);
        float halfPointSize = pointSize / 2f;
        
        linesVA.Resize((uint)(linesToDraw.Count * 6));
        
        for (int i = 0; i < linesToDraw.Count; i++)
        {
            var (line, isBlue) = linesToDraw[i];
            var color = isBlue ? Color.Blue : Color.Red;
            
            var center1 = camera.WorldToScreen(line.Point1.X, line.Point1.Y, windowWidth, windowHeight);
            var center2 = camera.WorldToScreen(line.Point2.X, line.Point2.Y, windowWidth, windowHeight);
            
            if (line.Point1.X == line.Point2.X)
            {
                float x = center1.X;
                
                float topPointY, bottomPointY;
                
                if (line.Point1.Y < line.Point2.Y)
                {
                    topPointY = center1.Y + halfPointSize;
                    bottomPointY = center2.Y - halfPointSize;
                }
                else
                {
                    topPointY = center2.Y + halfPointSize;
                    bottomPointY = center1.Y - halfPointSize;
                }
                
                float left = x - halfPointSize;
                float right = x + halfPointSize;
                
                AddQuadToVA(linesVA, (uint)(i * 6), left, right, topPointY, bottomPointY, color);
            }
            else
            {
                float y = center1.Y;
                
                float leftPointX, rightPointX;
                
                if (line.Point1.X < line.Point2.X)
                {
                    leftPointX = center1.X + halfPointSize;
                    rightPointX = center2.X - halfPointSize;
                }
                else
                {
                    leftPointX = center2.X + halfPointSize;
                    rightPointX = center1.X - halfPointSize;
                }
                
                float top = y - halfPointSize;
                float bottom = y + halfPointSize;
                
                AddQuadToVA(linesVA, (uint)(i * 6), leftPointX, rightPointX, top, bottom, color);
            }
        }
    }
    
    private void UpdateGridVA()
    {
        var (windowWidth, windowHeight) = window.Size;
        var (visibleWidth, visibleHeight) = camera.GetVisibleArea();

        float worldScale = Math.Min(
            (float)windowWidth / (float)visibleWidth,
            (float)windowHeight / (float)visibleHeight
        );

        float halfSize = (pointSizeInWorld * worldScale) / 2f;
        var gridPositions = camera.VisiblePoints.ToArray();

        gridVA.Resize((uint)(gridPositions.Length * 6));

        for (uint i = 0; i < gridPositions.Length; i++)
        {
            var (screenX, screenY) = camera.WorldToScreen(gridPositions[i].X, gridPositions[i].Y, windowWidth, windowHeight);

            float left = screenX - halfSize;
            float right = screenX + halfSize;
            float top = screenY - halfSize;
            float bottom = screenY + halfSize;
            AddQuadToVA(gridVA, i * 6, left, right, top, bottom, Color.White);
        }
    }
    
    public void DrawSelection()
    {
        (long x, long y) point1 = default;
        (long x, long y) point2 = default;

        var mousePos = Mouse.GetPosition(window);
        var worldCoords = camera.ScreenToWorld(mousePos.X, mousePos.Y, window.Size.X, window.Size.Y);

        double distX = Math.Abs(worldCoords.X - Math.Round(worldCoords.X));
        double distY = Math.Abs(worldCoords.Y - Math.Round(worldCoords.Y));

        long lowerX = (long)Math.Floor(worldCoords.X);
        long upperX = (long)Math.Ceiling(worldCoords.X);

        long lowerY = (long)Math.Floor(worldCoords.Y);
        long upperY = (long)Math.Ceiling(worldCoords.Y);

        if (distX < distY && distX < pointSizeInWorld)
        {
            long nearest = (long)Math.Round(worldCoords.X);
            point1 = (nearest, lowerY);
            point2 = (nearest, upperY);
        }

        if (distY < distX && distY < pointSizeInWorld)
        {
            long nearest = (long)Math.Round(worldCoords.Y);
            point1 = (lowerX, nearest);
            point2 = (upperX, nearest);
        }
        if (point1 == default && point2 == default)
            return;

        var (windowWidth, windowHeight) = window.Size;

        var (visibleWidth, visibleHeight) = camera.GetVisibleArea();

        float worldScale = Math.Min(
            (float)windowWidth / (float)visibleWidth,
            (float)windowHeight / (float)visibleHeight
        );
        float halfSize = (pointSizeInWorld * worldScale) / 2f;
        float thirdSize = (pointSizeInWorld * worldScale) / 3f;

        var p1 = camera.WorldToScreen(point1.x, point1.y, windowWidth, windowHeight);
        var p2 = camera.WorldToScreen(point2.x, point2.y, windowWidth, windowHeight);

        if (point1.y == point2.y)
        {
            AddQuadToVA(selectionVA, 00, p1.X + halfSize, p1.X + halfSize + thirdSize, p1.Y - halfSize, p1.Y - halfSize + thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 06, p1.X + halfSize, p1.X + halfSize + thirdSize / 2, p1.Y - halfSize, p1.Y - halfSize + thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 12, p1.X + halfSize, p1.X + halfSize + thirdSize, p1.Y + halfSize, p1.Y + halfSize - thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 18, p1.X + halfSize, p1.X + halfSize + thirdSize / 2, p1.Y + halfSize, p1.Y + halfSize - thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 24, p2.X - halfSize, p2.X - halfSize - thirdSize, p2.Y - halfSize, p2.Y - halfSize + thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 30, p2.X - halfSize, p2.X - halfSize - thirdSize / 2, p2.Y - halfSize, p2.Y - halfSize + thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 36, p2.X - halfSize, p2.X - halfSize - thirdSize, p2.Y + halfSize, p2.Y + halfSize - thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 42, p2.X - halfSize, p2.X - halfSize - thirdSize / 2, p2.Y + halfSize, p2.Y + halfSize - thirdSize, Color.Green);

            window.Draw(selectionVA);
            return;
        }
        if (point1.x == point2.x)
        {
            AddQuadToVA(selectionVA, 00, p1.X - halfSize, p1.X - halfSize + thirdSize, p1.Y + halfSize, p1.Y + halfSize + thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 06, p1.X - halfSize, p1.X - halfSize + thirdSize / 2, p1.Y + halfSize, p1.Y + halfSize + thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 12, p1.X + halfSize, p1.X + halfSize - thirdSize, p1.Y + halfSize, p1.Y + halfSize + thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 18, p1.X + halfSize, p1.X + halfSize - thirdSize / 2, p1.Y + halfSize, p1.Y + halfSize + thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 24, p2.X + halfSize, p2.X + halfSize - thirdSize, p2.Y - halfSize, p2.Y - halfSize - thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 30, p2.X + halfSize, p2.X + halfSize - thirdSize / 2, p2.Y - halfSize, p2.Y - halfSize - thirdSize, Color.Green);

            AddQuadToVA(selectionVA, 36, p2.X - halfSize, p2.X - halfSize + thirdSize, p2.Y - halfSize, p2.Y - halfSize - thirdSize / 2, Color.Green);
            AddQuadToVA(selectionVA, 42, p2.X - halfSize, p2.X - halfSize + thirdSize / 2, p2.Y - halfSize, p2.Y - halfSize - thirdSize, Color.Green);

            window.Draw(selectionVA);
            return;
        }
    }

    private static void AddQuadToVA(VertexArray vertexArray, uint begin, 
        float left, float right, float top, float bottom, 
        SFML.Graphics.Color color = default)
    {
        vertexArray[begin + 0] = new Vertex(new Vector2f(left, bottom), color);
        vertexArray[begin + 1] = new Vertex(new Vector2f(right, bottom), color);
        vertexArray[begin + 2] = new Vertex(new Vector2f(right, top), color);

        vertexArray[begin + 3] = new Vertex(new Vector2f(left, bottom), color);
        vertexArray[begin + 4] = new Vertex(new Vector2f(right, top), color);
        vertexArray[begin + 5] = new Vertex(new Vector2f(left, top), color);
    }
}