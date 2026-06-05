using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileGame.Core;
using TileGame.World;

namespace TileGame.Entities;

public class Player : Entity
{
    private const float Speed = 90f;            // pixels per second
    private const float AnimInterval = 16 / 60f; // toggle frame every 16 ticks at 60fps

    private readonly Texture2D _sheet;
    private Direction _facing = Direction.South;
    private int _animFrame;
    private float _animTimer;

    public Player(Vector2 startPosition, Texture2D sheet)
    {
        Position = startPosition;
        _sheet = sheet;
    }

    public void Update(InputState input, TileMap map, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var move = Vector2.Zero;

        if (input.IsDown(Keys.W) || input.IsDown(Keys.Up))    { move.Y -= 1; _facing = Direction.North; }
        if (input.IsDown(Keys.S) || input.IsDown(Keys.Down))  { move.Y += 1; _facing = Direction.South; }
        if (input.IsDown(Keys.A) || input.IsDown(Keys.Left))  { move.X -= 1; _facing = Direction.West;  }
        if (input.IsDown(Keys.D) || input.IsDown(Keys.Right)) { move.X += 1; _facing = Direction.East;  }

        if (move != Vector2.Zero)
        {
            if (move.LengthSquared() > 1f) move = Vector2.Normalize(move);
            TryMove(move * Speed * dt, map);

            _animTimer += dt;
            if (_animTimer >= AnimInterval)
            {
                _animTimer -= AnimInterval;
                _animFrame = (_animFrame + 1) % 2;
            }
        }
        else
        {
            _animFrame = 0;
            _animTimer = 0f;
        }
    }

    public void Draw(SpriteBatch sb)
    {
        var (row, flip) = _facing switch
        {
            Direction.South => (0, SpriteEffects.None),
            Direction.East  => (1, SpriteEffects.None),
            Direction.North => (2, SpriteEffects.None),
            Direction.West  => (1, SpriteEffects.FlipHorizontally),
            _ => (0, SpriteEffects.None)
        };

        var src = new Rectangle(_animFrame * TileMap.TileSize, row * TileMap.TileSize, TileMap.TileSize, TileMap.TileSize);
        sb.Draw(_sheet, Position, src, Color.White, 0f, Vector2.Zero, 1f, flip, 0f);
    }
}
