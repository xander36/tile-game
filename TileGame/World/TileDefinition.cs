using Microsoft.Xna.Framework;

namespace TileGame.World;

public readonly record struct TileDefinition(Rectangle SourceRect, bool IsSolid);
