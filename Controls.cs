using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Controls
{
    private RenderWindow window;
    private Camera camera;

    //пока что так, но надо будет исправить
    public Controls(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        window.KeyPressed += ZoomCamera;
        window.KeyPressed += MoveCamera;
        window.MouseButtonPressed += (object? sender, SFML.Window.MouseButtonEventArgs e) =>
        {
            Console.WriteLine($"Screen {e.X} {e.Y}");
            var worldCoords = GetWorldCoords((e.X, e.Y), window.Size);
            Console.WriteLine($"World {worldCoords.X} {worldCoords.Y}");
            var screenCoords = GetScreenCoords(worldCoords.X, worldCoords.Y, window.Size.X, window.Size.Y);
            Console.WriteLine($"Screen calculated {screenCoords.X} {screenCoords.Y}");
            (double X, double Y) dist = getCloseCoordDistance(worldCoords);
            double distY = Math.Abs(worldCoords.Y - Math.Round(worldCoords.Y));

            long lowerX = (long)Math.Floor(worldCoords.X);
            long upperX = (long)Math.Ceiling(worldCoords.X);

            long lowerY = (long)Math.Floor(worldCoords.Y);
            long upperY = (long)Math.Ceiling(worldCoords.Y);
            Console.WriteLine($"dist {dist.X} {dist.Y}");
            Console.WriteLine($"lowerX {lowerX} upperX {upperX} nearest {(long)Math.Round(worldCoords.X)}");
            Console.WriteLine($"lowerY {lowerY} upperY {upperY} nearest {(long)Math.Round(worldCoords.Y)}");
        };
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
            camera.Scale -= 0.5f;
            Thread.Sleep(5);
        }
        if (e.Code == SFML.Window.Keyboard.Key.Hyphen)
        {
            camera.Scale += 0.5f;
            Thread.Sleep(5);
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
