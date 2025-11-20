using SFML;
using SFML.System;
using SFML.Window;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal static class Program
{
    private static GameLogic gameLogic;
    
    private static void Main()
    {
        var mode = new SFML.Window.VideoMode(600, 600);
        var window = new SFML.Graphics.RenderWindow(mode, "SIAKOD game");
        window.SetFramerateLimit(60);
        
        Camera camera = new(scale: 2.25f, aspectRatio: 1f);
        Render render = new(window, camera);
        Controls controls = new(window, camera);
        gameLogic = new GameLogic(window, camera, controls, render, vsComputer: true);
        
        camera.CameraChanged.Invoke();

        window.Resized += (object? sender, SFML.Window.SizeEventArgs e) =>
        {
            camera.AspectRatio = window.Size.X / (float)window.Size.Y;
            SFML.Graphics.FloatRect visibleArea = new(0, 0, window.Size.X, window.Size.Y);
            window.SetView(new SFML.Graphics.View(visibleArea));
        };

        window.KeyPressed += (object? sender, SFML.Window.KeyEventArgs e) =>
        {
            if (e.Code == SFML.Window.Keyboard.Key.C)
            {
                bool currentMode = !gameLogic.GetVsComputer();
                gameLogic.SetVsComputer(currentMode);
                Console.WriteLine($"Mode changed: {(currentMode ? "VS Computer" : "Two Players")}");
            }
        };

        var drawSelection = () =>
        {
            var mousePos = Mouse.GetPosition(window);
            var worldCoords = camera.ScreenToWorld(mousePos.X, mousePos.Y, window.Size.X, window.Size.Y);
        
            double distX = Math.Abs(worldCoords.X - Math.Round(worldCoords.X));
            double distY = Math.Abs(worldCoords.Y - Math.Round(worldCoords.Y));

            long lowerX = (long)Math.Floor(worldCoords.X);
            long upperX = (long)Math.Ceiling(worldCoords.X);

            long lowerY = (long)Math.Floor(worldCoords.Y);
            long upperY = (long)Math.Ceiling(worldCoords.Y);

            if (distX < distY && distX < 0.1f)
            {
                long nearest = (long)Math.Round(worldCoords.X);
                render.DrawSelection((nearest, lowerY), (nearest, upperY));
                return;
            }
        
            if (distY < distX && distY < 0.1f)
            {
                long nearest = (long)Math.Round(worldCoords.Y);
                render.DrawSelection((lowerX, nearest), (upperX, nearest));
                return;
            }
        };
        drawSelection.Invoke();

        window.Closed += (object? sender, EventArgs e) => window.Close();

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(new(30, 30, 30));
            
            render.DrawGrid();
            render.DrawLines();
            drawSelection.Invoke();
            window.Display();
            
            if (gameLogic.CheckForBlueWin())
            {
                Console.WriteLine("BLUE WINS");
                window.Close();
            }
        }

        Console.WriteLine("Goodbye World");
    }
}