using Microsoft.Xna.Framework;
using TileGame.World;

namespace TileGame.Entities;

public abstract class Entity
{
    public Vector2 Position { get; protected set; }

    // 1px inset on each side so the player doesn't snag on corners of adjacent tiles
    private Rectangle BoundsAt(Vector2 pos) => new(
        (int)pos.X + 1, (int)pos.Y + 1,
        TileMap.TileSize - 2, TileMap.TileSize - 2);

    protected bool TryMove(Vector2 delta, TileMap map)
    {
        var full = Position + delta;
        if (!Overlaps(full, map)) { Position = full; return true; }

        var xOnly = new Vector2(Position.X + delta.X, Position.Y);
        if (!Overlaps(xOnly, map)) { Position = xOnly; return true; }

        var yOnly = new Vector2(Position.X, Position.Y + delta.Y);
        if (!Overlaps(yOnly, map)) { Position = yOnly; return true; }

        return false;
    }

    private bool Overlaps(Vector2 pos, TileMap map)
    {
        var b = BoundsAt(pos);
        int left   = b.Left   / TileMap.TileSize;
        int right  = (b.Right  - 1) / TileMap.TileSize;
        int top    = b.Top    / TileMap.TileSize;
        int bottom = (b.Bottom - 1) / TileMap.TileSize;

        return map.IsSolid(left, top)  || map.IsSolid(right, top) ||
               map.IsSolid(left, bottom) || map.IsSolid(right, bottom);
    }
}
