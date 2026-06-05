using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileGame.Core;
using TileGame.Entities;
using TileGame.World;

namespace TileGame;

public class Game1 : Game
{
    private const float Scale     = 3f;
    private const int ViewTilesW  = 20;
    private const int ViewTilesH  = 12;
    private const int WindowW     = (int)(ViewTilesW * TileMap.TileSize * Scale); // 960
    private const int WindowH     = (int)(ViewTilesH * TileMap.TileSize * Scale); // 576

    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly InputState _input  = new();
    private readonly Camera     _camera = new();

    private TileMap _map;
    private Player _player;
    private List<Seeker> _enemies;

    private Texture2D _dungeonSheet;
    private Texture2D _oozeSheet;
    private Texture2D _ratSheet;

    private readonly Random _rng = new();

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
        _spriteBatch  = new SpriteBatch(GraphicsDevice);
        _dungeonSheet = Content.Load<Texture2D>("dungeon_tiles");
        _oozeSheet    = Content.Load<Texture2D>("ooze_sprites");
        _ratSheet     = Content.Load<Texture2D>("grey_rat_sprites");
        var playerSheet = Content.Load<Texture2D>("human_sprites");

        var (map, spawn, enemySpawns) = MapLoader.Load("Content/Maps/dungeon_0");
        _map    = map;
        _player = new Player(spawn, playerSheet);

        _enemies = new List<Seeker>();
        foreach (var e in enemySpawns)
        {
            var (sheet, health, speed) = e.Type switch
            {
                'r' or 'R' => (_ratSheet,  2, 65f),
                _          => (_oozeSheet, 3, 50f),
            };
            _enemies.Add(new Seeker(e.Position, sheet, health, damage: 1, speed, _rng));
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        _input.Update();
        _player.Update(_input, _map, _enemies, gameTime);

        foreach (var enemy in _enemies)
            enemy.Update(_player, _map, gameTime);

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

        foreach (var enemy in _enemies)
            enemy.Draw(_spriteBatch);

        _player.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
