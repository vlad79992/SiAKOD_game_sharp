using System;
using SFML;

internal static class Program
{
    private static void Main()
    {
        var mode = new SFML.Window.VideoMode(800, 600);
        var window = new SFML.Graphics.RenderWindow(mode, "SIAKOD game");
        window.SetFramerateLimit(60);
        
        Camera camera = new(scale: 20, aspectRatio: 800.0f / 600.0f);
        Render render = new(window, camera);
        camera.CameraChanged.Invoke(); // обновляем массив вершин в Render

        window.Resized += (object? sender, SFML.Window.SizeEventArgs e) => camera.AspectRatio = window.Size.X / (float)window.Size.Y;
        window.Closed += (object? sender, EventArgs e) => window.Close();

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(new(30, 30, 30));
            
            render.DrawGrid();
            
            window.Display();
        }
    }
}