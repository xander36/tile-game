using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileGame.Entities;

namespace TileGame.UI;

public class Hud
{
    private const int HeartSize  = 14;
    private const int HeartGap   = 4;
    private const int Margin     = 10;

    private readonly Texture2D _pixel;

    public Hud(GraphicsDevice gd)
    {
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch sb, Player player, int screenW, int screenH)
    {
        for (int i = 0; i < player.MaxHealth; i++)
        {
            var rect = new Rectangle(
                Margin + i * (HeartSize + HeartGap),
                Margin,
                HeartSize, HeartSize);

            var color = i < player.Health ? new Color(200, 30, 30) : new Color(60, 20, 20);
            sb.Draw(_pixel, rect, color);
        }
    }

    public void DrawDeathOverlay(SpriteBatch sb, float alpha, int screenW, int screenH)
    {
        var overlay = new Color(180, 0, 0, (int)(alpha * 200));
        sb.Draw(_pixel, new Rectangle(0, 0, screenW, screenH), overlay);
    }
}
