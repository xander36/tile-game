using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileGame.World;

namespace TileGame.Entities;

public class Seeker : Creature
{
    private const float AnimInterval  = 16f / 60f;
    private const float WanderInterval = 16f / 60f;
    private const float ScanInterval  = 0.8f;
    private const float AggroRange    = 96f;
    private const float AttackRange   = 20f;
    private const float AttackCooldown = 1.0f;

    private readonly Texture2D _sheet;
    private readonly Random _rng;
    private readonly float _speed;

    private Direction _facing = Direction.South;
    private int _animFrame;
    private float _animTimer;

    private bool _hasTarget;
    private float _scanTimer;
    private float _attackTimer;
    private float _wanderTimer;
    private Direction _wanderDir;

    public int Damage { get; }

    public Seeker(Vector2 position, Texture2D sheet, int health, int damage, float speed, Random rng)
        : base(health)
    {
        Position  = position;
        _sheet    = sheet;
        Damage    = damage;
        _speed    = speed;
        _rng      = rng;
        _wanderDir = (Direction)rng.Next(4);
        _facing   = _wanderDir;
    }

    public void Update(Player player, TileMap map, GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        TickTimers(dt);
        _attackTimer = Math.Max(0f, _attackTimer - dt);

        if (IsDead || IsFrozen) return;

        _scanTimer -= dt;
        if (_scanTimer <= 0f)
        {
            _scanTimer = ScanInterval;
            _hasTarget = !player.IsDead &&
                         Vector2.Distance(Position, player.Position) <= AggroRange;
        }

        if (_hasTarget)
        {
            var toPlayer = player.Position - Position;
            var dist = toPlayer.Length();

            if (dist <= AttackRange)
            {
                if (_attackTimer <= 0f)
                {
                    _attackTimer = AttackCooldown;
                    player.TakeDamage(Damage, Vector2.Normalize(toPlayer), map);
                }
            }
            else
            {
                Chase(toPlayer, dt, map);
                _facing = MathF.Abs(toPlayer.X) >= MathF.Abs(toPlayer.Y)
                    ? (toPlayer.X > 0 ? Direction.East : Direction.West)
                    : (toPlayer.Y > 0 ? Direction.South : Direction.North);
            }
        }
        else
        {
            Wander(dt, map);
        }

        _animTimer += dt;
        if (_animTimer >= AnimInterval)
        {
            _animTimer -= AnimInterval;
            _animFrame = (_animFrame + 1) % 2;
        }
    }

    private void Chase(Vector2 toPlayer, float dt, TileMap map)
    {
        var sx = MathF.Sign(toPlayer.X);
        var sy = MathF.Sign(toPlayer.Y);
        if (_rng.Next(2) == 0)
        {
            if (!TryMove(new Vector2(sx, 0) * _speed * dt, map))
                TryMove(new Vector2(0, sy) * _speed * dt, map);
        }
        else
        {
            if (!TryMove(new Vector2(0, sy) * _speed * dt, map))
                TryMove(new Vector2(sx, 0) * _speed * dt, map);
        }
    }

    private void Wander(float dt, TileMap map)
    {
        _wanderTimer -= dt;
        if (_wanderTimer <= 0f)
        {
            _wanderTimer = WanderInterval + (float)_rng.NextDouble() * 0.8f;
            _wanderDir = (Direction)_rng.Next(4);
            _facing = _wanderDir;
        }

        var dir = _wanderDir switch
        {
            Direction.North => new Vector2(0, -1),
            Direction.South => new Vector2(0,  1),
            Direction.East  => new Vector2( 1, 0),
            Direction.West  => new Vector2(-1, 0),
            _ => Vector2.Zero
        };
        TryMove(dir * _speed * 0.4f * dt, map);
    }

    public void Draw(SpriteBatch sb)
    {
        if (IsDead) return;

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
