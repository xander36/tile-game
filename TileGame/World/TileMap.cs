using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileGame.World;

public class TileMap
{
    public const int TileSize = 16;

    private readonly char[,] _layer0;
    private readonly char[,] _layer1;
    private readonly IReadOnlyDictionary<char, TileDefinition> _defs;

    public int Width { get; }
    public int Height { get; }

    public TileMap(char[,] layer0, char[,] layer1, IReadOnlyDictionary<char, TileDefinition> defs)
    {
        _layer0 = layer0;
        _layer1 = layer1;
        _defs = defs;
        Height = layer0.GetLength(0);
        Width = layer0.GetLength(1);
    }

    public bool IsSolid(int tileX, int tileY)
    {
        if (tileX < 0 || tileY < 0 || tileX >= Width || tileY >= Height)
            return true;

        return (_defs.TryGetValue(_layer1[tileY, tileX], out var d1) && d1.IsSolid) ||
               (_defs.TryGetValue(_layer0[tileY, tileX], out var d0) && d0.IsSolid);
    }

    public void Draw(SpriteBatch sb, Texture2D sheet, Vector2 cameraPos, int viewPixelW, int viewPixelH)
    {
        int startX = Math.Max(0, (int)(cameraPos.X / TileSize) - 1);
        int startY = Math.Max(0, (int)(cameraPos.Y / TileSize) - 1);
        int endX = Math.Min(Width,  startX + viewPixelW / TileSize + 3);
        int endY = Math.Min(Height, startY + viewPixelH / TileSize + 3);

        DrawLayer(_layer0, sb, sheet, startX, startY, endX, endY);
        DrawLayer(_layer1, sb, sheet, startX, startY, endX, endY);
    }

    private void DrawLayer(char[,] layer, SpriteBatch sb, Texture2D sheet, int x0, int y0, int x1, int y1)
    {
        for (int y = y0; y < y1; y++)
        {
            for (int x = x0; x < x1; x++)
            {
                if (!_defs.TryGetValue(layer[y, x], out var def)) continue;
                sb.Draw(sheet, new Vector2(x * TileSize, y * TileSize), def.SourceRect, Color.White);
            }
        }
    }
}
