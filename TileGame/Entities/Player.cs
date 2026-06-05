using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileGame.Core;
using TileGame.World;

namespace TileGame.Entities;

public class Player : Creature
{
    private const float Speed          = 90f;
    private const float AnimInterval   = 16f / 60f;
    private const float AttackDuration = 0.25f;
    private const float AttackRange    = 26f;
    private const int   AttackDamage   = 1;

    private readonly Texture2D _sheet;
    private Direction _facing = Direction.South;
    private int _animFrame;
    private float _animTimer;

    private readonly Vector2 _spawnPosition;

    public Player(Vector2 startPosition, Texture2D sheet) : base(maxHealth: 5)
    {
        _spawnPosition = startPosition;
        Position = startPosition;
        _sheet   = sheet;
    }

    public void Respawn() => Respawn(_spawnPosition);

    public void Update(InputState input, TileMap map, IReadOnlyList<Seeker> enemies, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TickTimers(dt);

        if (!IsFrozen)
        {
            HandleMovement(input, map, dt);

            if (input.JustPressed(Keys.Space) || input.JustPressed(Keys.Z))
            {
                FreezeTimer = AttackDuration;
                Attack(enemies, map);
            }
        }
        else
        {
            _animFrame = 0;
            _animTimer = 0f;
        }
    }

    private void HandleMovement(InputState input, TileMap map, float dt)
    {
        var move = Vector2.Zero;
        if (input.IsDown(Keys.W) || input.IsDown(Keys.Up))    { move.Y -= 1; _facing = Direction.North; }
        if (input.IsDown(Keys.S) || input.IsDown(Keys.Down))  { move.Y += 1; _facing = Direction.South; }
        if (input.IsDown(Keys.A) || input.IsDown(Keys.Left))  { move.X -= 1; _facing = Direction.West;  }
        if (input.IsDown(Keys.D) || input.IsDown(Keys.Right)) { move.X += 1; _facing = Direction.East;  }

        if (move == Vector2.Zero)
        {
            _animFrame = 0;
            _animTimer = 0f;
            return;
        }

        if (move.LengthSquared() > 1f) move = Vector2.Normalize(move);
        TryMove(move * Speed * dt, map);

        _animTimer += dt;
        if (_animTimer >= AnimInterval)
        {
            _animTimer -= AnimInterval;
            _animFrame = (_animFrame + 1) % 2;
        }
    }

    private void Attack(IReadOnlyList<Seeker> enemies, TileMap map)
    {
        var dir = _facing switch
        {
            Direction.North => new Vector2( 0, -1),
            Direction.South => new Vector2( 0,  1),
            Direction.East  => new Vector2( 1,  0),
            Direction.West  => new Vector2(-1,  0),
            _ => new Vector2(0, 1)
        };

        var hitCenter = Position + new Vector2(TileMap.TileSize * 0.5f) + dir * TileMap.TileSize;

        foreach (var enemy in enemies)
        {
            if (enemy.IsDead) continue;
            var enemyCenter = enemy.Position + new Vector2(TileMap.TileSize * 0.5f);
            if (Vector2.Distance(hitCenter, enemyCenter) <= AttackRange)
                enemy.TakeDamage(AttackDamage, dir, map);
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
        sb.Draw(_sheet, Position, src, DrawColor, 0f, Vector2.Zero, 1f, flip, 0f);
    }
}
