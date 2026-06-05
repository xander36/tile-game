using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TileGame.World;

public static class MapLoader
{
    public static readonly IReadOnlyDictionary<char, TileDefinition> DungeonDefs =
        new Dictionary<char, TileDefinition>
        {
            ['f'] = new(new Rectangle( 0,  0, 16, 16), IsSolid: false),
            ['d'] = new(new Rectangle( 0, 16, 16, 16), IsSolid: true),
            ['D'] = new(new Rectangle(16, 16, 16, 16), IsSolid: true),
        };

    public static (TileMap Map, Vector2 PlayerSpawn) Load(string directory)
    {
        var l0Lines = File.ReadAllLines(Path.Combine(directory, "layer0.txt"));
        var l1Lines = File.ReadAllLines(Path.Combine(directory, "layer1.txt"));

        int height = Math.Min(l0Lines.Length, l1Lines.Length);
        int width = l0Lines.Concat(l1Lines).Max(l => l.Length);

        var layer0 = new char[height, width];
        var layer1 = new char[height, width];
        var playerSpawn = Vector2.Zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                layer0[y, x] = x < l0Lines[y].Length ? l0Lines[y][x] : '*';

                char c = x < l1Lines[y].Length ? l1Lines[y][x] : '*';
                if (c == '@')
                {
                    playerSpawn = new Vector2(x * TileMap.TileSize, y * TileMap.TileSize);
                    c = '*';
                }
                layer1[y, x] = c;
            }
        }

        return (new TileMap(layer0, layer1, DungeonDefs), playerSpawn);
    }
}
