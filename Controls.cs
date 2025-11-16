using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

internal class Controls
{
    private SFML.Graphics.RenderWindow window;
    private Camera camera;

    //пока что так, но надо будет исправить
    public Controls(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
        window.KeyPressed += ZoomCamera;
        window.KeyPressed += MoveCamera;
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
            camera.Position = new(camera.Position.x - vel, camera.Position.y);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Right)
        {
            camera.Position = new(camera.Position.x + vel, camera.Position.y);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Up)
        {
            camera.Position = new(camera.Position.x, camera.Position.y + vel);
            keyPressed = true;
        }
        if (e.Code == SFML.Window.Keyboard.Key.Down)
        {
            camera.Position = new(camera.Position.x, camera.Position.y - vel);
            keyPressed = true;
        }
        if (keyPressed)
            Thread.Sleep(10);
    }
}
