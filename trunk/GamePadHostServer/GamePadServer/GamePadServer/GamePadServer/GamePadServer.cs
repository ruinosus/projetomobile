using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GamePadServer
{
    class GamepadServer
    {
        HttpListener listener;
        GamePadServer hostGame;

        public void RunServer(GamePadServer inHostGame)
        {
            hostGame = inHostGame;
            Thread server = new Thread(serverListener);
            server.Start();
        }

        private void serverListener()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                new Thread(new serverThread(context, hostGame).ProcessRequest).Start();
            }
        }

        class serverThread
        {
            private HttpListenerContext context;
            private GamePadServer game;

            public serverThread(HttpListenerContext inContext, GamePadServer inGame)
            {
                context = inContext;
                game = inGame;
            }

            public void ProcessRequest()
            {
                StringBuilder result = new StringBuilder();

                result.Append(game.AccelX.ToString());
                result.Append(";");
                result.Append(game.AccelY.ToString());
                result.Append(";");
                result.Append(game.AccelZ.ToString());
                result.Append(";");
                result.Append(game.ButtonA.ToString());

                byte[] b = Encoding.UTF8.GetBytes(result.ToString());
                context.Response.ContentLength64 = b.Length;
                context.Response.OutputStream.Write(b, 0, b.Length);
                context.Response.OutputStream.Close();
            }
        }
    }


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GamePadServer : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GamepadServer server;

        public float AccelX;
        public float AccelY;
        public float AccelZ;
        public bool ButtonA;

        SpriteFont font;

        public GamePadServer()
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
            server = new GamepadServer();

            server.RunServer(this);

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

            font = Content.Load<SpriteFont>("Font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            GamePadState state = GamePad.GetState(PlayerIndex.One);

            AccelX = state.ThumbSticks.Left.X;
            AccelY = state.ThumbSticks.Left.Y;
            AccelZ = state.ThumbSticks.Right.X;
            ButtonA = state.IsButtonDown(Buttons.A);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "X: " + AccelX + " Y: " + AccelY + " Z: " + AccelZ + " A: " + ButtonA, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
