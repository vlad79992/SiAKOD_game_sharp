using SFML;
using SFML.System;
using SFML.Window;

internal static class Program
{   
    private static void Main()
    {
        var mode = new SFML.Window.VideoMode(600, 600);
        var window = new SFML.Graphics.RenderWindow(mode, "SIAKOD game");
        window.SetFramerateLimit(60);

        Camera camera = new(scale: 2.25f, aspectRatio: 1f);
        Render render = new(window, camera);
        Controls controls = new(window, camera);
        GameLogic gameLogic = new GameLogic(window, camera, controls, render, vsComputer: true);

        camera.CameraChanged?.Invoke();

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
                gameLogic.VsComputer = !gameLogic.VsComputer;
                Console.WriteLine($"Mode changed: {(gameLogic.VsComputer ? "VS Computer" : "Two Players")}");
            }
        };

        window.Closed += (object? sender, EventArgs e) => window.Close();

        ShowTutorial(window);
        GameRenderLoop(window, render, gameLogic);
        ShowEnding(window);

        Console.WriteLine("Goodbye World");
    }

    private static void GameRenderLoop(SFML.Graphics.RenderWindow window, Render render, GameLogic gameLogic)
    {
        SFML.Graphics.Font font = new("Monocraft-nerd-fonts-patched.ttc");

        SFML.Graphics.Text text = new("ЧЛЕЕЕН", font, 256);
        text.FillColor = SFML.Graphics.Color.White;
        text.Position = new Vector2f(100, 100);

        while (window.IsOpen && !gameLogic.BlueWins)
        {
            window.DispatchEvents();
            window.Clear(new(30, 30, 30));

            render.DrawGrid();
            render.DrawLines();
            render.DrawSelection();
            window.Draw(text);
            window.Display();
        }
    }
    private static void ShowEnding(SFML.Graphics.RenderWindow window)
    {
        SFML.Graphics.Font font = new("Monocraft-nerd-fonts-patched.ttc");

        SFML.Graphics.Text text = new("ГОЛУБЫЕ ПОБЕДИЛИ", font, 100);
        text.FillColor = SFML.Graphics.Color.Blue;
        text.Position = new Vector2f(100, 100);

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(new(30, 30, 30));
            window.Draw(text);
            window.Display();
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                if (key != Keyboard.Key.Unknown && Keyboard.IsKeyPressed(key))
                {
                    break;
                }
            }
        }
    }
    private static void ShowTutorial(SFML.Graphics.RenderWindow window)
    {
        SFML.Graphics.Font font = new("Monocraft-nerd-fonts-patched.ttc");

        SFML.Graphics.Text text = new(
            "Нажмите клавишу C,\nчтобы переместиться в Қазақстан,\n"
            + "а также сменить режим игры.\n"
            + "\tДля начала нажмите любую кнопку.",
            font,
            60);
        text.FillColor = SFML.Graphics.Color.White;
        text.Position = new Vector2f(100, 100);

        while (window.IsOpen)
        {
            window.DispatchEvents();
            window.Clear(new(30, 30, 30));
            window.Draw(text);
            window.Display();
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)))
            {
                if (key != Keyboard.Key.Unknown && Keyboard.IsKeyPressed(key))
                {
                    return;
                }
            }
        }
    }
}