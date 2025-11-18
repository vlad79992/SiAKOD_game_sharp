using System;
using SFML;
using SFML.Graphics;
using SFML.Window;

internal class GameLogic
{
    private RenderWindow window;
    private Camera camera;
    public GameLogic(RenderWindow window, Camera camera)
    {
        this.window = window;
        this.camera = camera;
    }

    
}

