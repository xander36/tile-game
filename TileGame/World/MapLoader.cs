using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TileGame.World;

public record EnemySpawn(Vector2 Position, char Type);

public static class MapLoader
{
    public static readonly IReadOnlyDictionary<char, TileDefinition> DungeonDefs =
        new Dictionary<char, TileDefinition>
        {
            ['f'] = new(new Rectangle( 0,  0, 16, 16), IsSolid: false),
            ['d'] = new(new Rectangle( 0, 16, 16, 16), IsSolid: true),
            ['D'] = new(new Rectangle(16, 16, 16, 16), IsSolid: true),
        };

    private static readonly HashSet<char> EnemyChars = new() { 'o', '0', 'r', 'R' };

    public static (TileMap Map, Vector2 PlayerSpawn, IReadOnlyList<EnemySpawn> Enemies) Load(string directory)
    {
        var l0Lines = File.ReadAllLines(Path.Combine(directory, "layer0.txt"));
        var l1Lines = File.ReadAllLines(Path.Combine(directory, "layer1.txt"));

        int height = Math.Min(l0Lines.Length, l1Lines.Length);
        int width  = l0Lines.Concat(l1Lines).Max(l => l.Length);

        var layer0  = new char[height, width];
        var layer1  = new char[height, width];
        var spawn   = Vector2.Zero;
        var enemies = new List<EnemySpawn>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                layer0[y, x] = x < l0Lines[y].Length ? l0Lines[y][x] : '*';

                char c = x < l1Lines[y].Length ? l1Lines[y][x] : '*';
                var worldPos = new Vector2(x * TileMap.TileSize, y * TileMap.TileSize);

                if (c == '@')
                {
                    spawn = worldPos;
                    c = '*';
                }
                else if (EnemyChars.Contains(c))
                {
                    enemies.Add(new EnemySpawn(worldPos, c));
                    c = '*';
                }

                layer1[y, x] = c;
            }
        }

        return (new TileMap(layer0, layer1, DungeonDefs), spawn, enemies);
    }
}
