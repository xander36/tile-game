using Microsoft.Xna.Framework.Input;

namespace TileGame.Core;

public class InputState
{
    private KeyboardState _current;
    private KeyboardState _previous;

    public void Update()
    {
        _previous = _current;
        _current = Keyboard.GetState();
    }

    public bool IsDown(Keys key) => _current.IsKeyDown(key);
    public bool JustPressed(Keys key) => _current.IsKeyDown(key) && !_previous.IsKeyDown(key);
}
