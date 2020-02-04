using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Timers;

namespace MonoGameWindowsStarter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private class Enemy
        {
            private bool alive = true;
            private Rectangle position = new Rectangle(2000, 0, 150, 150);
            private int speed;

            public Enemy(int speed, int YPosition)
            {
                this.speed = speed;
                position.Y = YPosition;
            }

            public bool Touches(Rectangle player)
            {
                return !(position.X > player.X + player.Width
                    || position.X + position.Width < player.X
                    || position.Y > player.Y + player.Height
                    || position.Y + position.Height < player.Y);
            }

            public int Speed { get => speed; }
            public bool Alive { get => alive; set => alive = value; }
            public Rectangle Position { get => position; set => position = value; }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerSprite;
        Rectangle playerPosition;
        Texture2D enemySprite;
        List<Enemy> enemies;
        int enemySpeed = 8;
        SpriteFont scoreText;
        Random random;
        Timer timer;
        int score = 0;
        Vector2 scorePosition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
            random = new Random();

            timer = new Timer(1000);
            timer.Elapsed += AddEnemy;
            timer.Start();

            scorePosition = new Vector2(GraphicsDevice.Viewport.Width - 200, 50);

            enemies = new List<Enemy>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            enemySprite = Content.Load<Texture2D>("piranha");
            playerSprite = Content.Load<Texture2D>("fish");
            scoreText = Content.Load<SpriteFont>("Score");
            playerPosition = new Rectangle(50, GraphicsDevice.Viewport.Height / 2, playerSprite.Width / 11, playerSprite.Height / 11);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void AddEnemy(object source, ElapsedEventArgs e)
        {
            // chooses when to speed up the enemies
            if (score % 5 == 0 && enemySpeed < 200)
            {
                enemySpeed++;

                if (timer.Interval > 50)
                {
                    timer.Stop();
                    timer.Interval -= 50;
                    timer.Start();
                }
            }
            enemies.Add(new Enemy(enemySpeed, random.Next(-20, 980)));
        }

        private void EndGame()
        {
            timer.Stop();
            enemies = new List<Enemy>();
            scorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 - 50, GraphicsDevice.Viewport.Height / 2 - 20);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.Alive)
                {
                    enemy.Position = new Rectangle(enemy.Position.X - enemy.Speed, enemy.Position.Y, enemy.Position.Width, enemy.Position.Height);

                    if (enemy.Position.X < -(enemySprite.Width - 20))
                    {
                        enemy.Alive = false;
                        score += 1;
                    }
                    else if (enemy.Touches(playerPosition))
                    {
                        EndGame();
                    }
                }
                else
                {
                    enemies.RemoveAt(i);
                }
            }

            // player position
            var playerSpeed = 15;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                playerPosition.Y -= playerSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                playerPosition.Y += playerSpeed;
            }

            if (playerPosition.Y < 0)
            {
                playerPosition.Y = 0;
            }
            if (playerPosition.Y > GraphicsDevice.Viewport.Height - playerPosition.Height)
            {
                playerPosition.Y = GraphicsDevice.Viewport.Height - playerPosition.Height;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.Alive)
                {
                    spriteBatch.Draw(enemySprite, new Rectangle(enemy.Position.X, enemy.Position.Y, 150, 150), Color.White);
                }
            }

            spriteBatch.Draw(playerSprite, playerPosition, Color.White);

            spriteBatch.DrawString(scoreText, score.ToString(), scorePosition, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
