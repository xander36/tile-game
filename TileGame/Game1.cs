using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileGame.Core;
using TileGame.Entities;
using TileGame.World;

namespace TileGame;

public class Game1 : Game
{
    private const float Scale = 3f;
    private const int ViewTilesW = 20;
    private const int ViewTilesH = 12;
    private const int WindowW = (int)(ViewTilesW * TileMap.TileSize * Scale); // 960
    private const int WindowH = (int)(ViewTilesH * TileMap.TileSize * Scale); // 576

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly InputState _input = new();
    private readonly Camera _camera = new();

    private TileMap _map;
    private Player _player;
    private Texture2D _dungeonSheet;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth  = WindowW,
            PreferredBackBufferHeight = WindowH,
        };
        Content.RootDirectory = "Content";
        Window.Title = "TileGame";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _dungeonSheet    = Content.Load<Texture2D>("dungeon_tiles");
        var playerSheet  = Content.Load<Texture2D>("human_sprites");

        var (map, spawn) = MapLoader.Load("Content/Maps/dungeon_0");
        _map    = map;
        _player = new Player(spawn, playerSheet);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        _input.Update();
        _player.Update(_input, _map, gameTime);
        _camera.Follow(
            _player.Position,
            new Vector2(WindowW / Scale, WindowH / Scale),
            (float)gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: _camera.GetTransform(Scale));

        _map.Draw(_spriteBatch, _dungeonSheet, _camera.Position, WindowW / (int)Scale, WindowH / (int)Scale);
        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
