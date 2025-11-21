using SFML.Graphics;
using SFML.System;

internal class Controls
{
    private readonly RenderWindow window;
    private readonly Camera camera;

    public Controls(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        window.KeyPressed += ZoomCamera;
        window.KeyPressed += MoveCamera;
    }

    public (double X, double Y) GetWorldCoords((int X, int Y) e, Vector2u windowSize)
    {
        return camera.ScreenToWorld(e.X, e.Y, windowSize.X, windowSize.Y);
    }

    public (double X, double Y) GetScreenCoords(double worldCoordsX, double worldCoordsY, uint windowSizeX, uint windowSizeY)
    {
        return camera.WorldToScreen(worldCoordsX, worldCoordsY, windowSizeX, windowSizeY);
    }

    public (double X, double Y) getCloseCoordDistance((double X, double Y) worldCoords)
    {
        return (Math.Abs(worldCoords.X - Math.Round(worldCoords.X)), Math.Abs(worldCoords.Y - Math.Round(worldCoords.Y)));
    }

    private async void ZoomCamera(object? sender, SFML.Window.KeyEventArgs e)
    {
        if (e.Code == SFML.Window.Keyboard.Key.Equal)
        {
            if (camera.Scale > 1)
            {
                camera.Scale -= 0.5f;
                Thread.Sleep(5);
            }
        }
        if (e.Code == SFML.Window.Keyboard.Key.Hyphen)
        {
            if (camera.Scale < 100)
            {
                camera.Scale += 0.5f;
                Thread.Sleep(5);
            }
        }
    }

    private async void MoveCamera(object? sender, SFML.Window.KeyEventArgs e)
    {
        float vel = 0.1f;
        bool keyPressed = false;
        if (e.Code == SFML.Window.Keyboard.Key.Left)
        {
            camera.Position = new(camera.Position.X - vel, camera.Position.Y);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Right)
        {
            camera.Position = new(camera.Position.X + vel, camera.Position.Y);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Up)
        {
            camera.Position = new(camera.Position.X, camera.Position.Y - vel);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Down)
        {
            camera.Position = new(camera.Position.X, camera.Position.Y + vel);
            keyPressed = true;
        }
        if (keyPressed)
            Thread.Sleep(10);
    }
}