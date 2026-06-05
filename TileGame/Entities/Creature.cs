using System;
using Microsoft.Xna.Framework;
using TileGame.World;

namespace TileGame.Entities;

public abstract class Creature : Entity
{
    public int Health { get; private set; }
    public int MaxHealth { get; }
    public bool IsDead => Health <= 0;

    protected float FreezeTimer;
    public bool IsFrozen => FreezeTimer > 0;

    private float _hurtFlash;
    protected Color DrawColor => _hurtFlash > 0 ? Color.Red : Color.White;

    protected Creature(int maxHealth)
    {
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

    public void TakeDamage(int damage, Vector2 knockbackDir, TileMap map)
    {
        if (IsDead) return;
        Health = Math.Max(0, Health - damage);
        _hurtFlash = 0.25f;
        FreezeTimer = Math.Max(FreezeTimer, 0.2f);
        for (int i = 0; i < 10; i++)
            TryMove(knockbackDir, map);
    }

    protected void TickTimers(float dt)
    {
        FreezeTimer = Math.Max(0f, FreezeTimer - dt);
        _hurtFlash  = Math.Max(0f, _hurtFlash  - dt);
    }
}
