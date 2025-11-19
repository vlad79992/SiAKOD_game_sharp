using System;
using SFML;
using SFML.Graphics;
using SFML.Window;

internal class GameLogic
{
    private RenderWindow window;
    private Camera camera;
    private Controls controls;
    public GameLogic(RenderWindow window, Camera camera, Controls controls)
    {
        this.window = window;
        this.camera = camera;
        this.controls = controls;
    }
    
    
    
}

