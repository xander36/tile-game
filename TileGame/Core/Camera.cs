using System;
using Microsoft.Xna.Framework;

namespace TileGame.Core;

public class Camera
{
    public Vector2 Position { get; private set; }

    public void Follow(Vector2 target, Vector2 worldViewSize, float dt)
    {
        var ideal = target - worldViewSize / 2f;
        Position += (ideal - Position) * Math.Min(1f, 8f * dt);
    }

    public Matrix GetTransform(float scale) =>
        Matrix.CreateTranslation(-Position.X, -Position.Y, 0f) *
        Matrix.CreateScale(scale, scale, 1f);
}
