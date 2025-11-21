using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

internal class Controls
{
    private readonly RenderWindow window;
    private readonly Camera camera;
    private Vector2i lastMousePos = new();
    public Controls(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        window.KeyPressed += ZoomCamera;
        window.KeyPressed += MoveCamera;
        window.MouseMoved += MoveCameraByMouse;
        window.MouseWheelScrolled += ZoomCameraByMouse;
    }

    private void ZoomCameraByMouse(object? sender, MouseWheelScrollEventArgs e)
    {
        float scrollSpeed = -0.25f;
        if (camera.Scale + e.Delta * scrollSpeed > 1 && camera.Scale + e.Delta * scrollSpeed < 100)
            camera.Scale += e.Delta * scrollSpeed;
    }

    private void MoveCameraByMouse(object? sender, MouseMoveEventArgs e)
    {
        float scale = 0.1f / camera.Scale;
        if (Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
        {
            Vector2i currentMousePos = new(e.X, e.Y);
            var delta = currentMousePos - lastMousePos;
            camera.Position = new(camera.Position.X + delta.X * scale, camera.Position.Y + delta.Y * scale);
        }
        lastMousePos = new(e.X, e.Y);
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
    private void MoveCamera(object? sender, SFML.Window.KeyEventArgs e)
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