using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace WindowsPhonePlatformer
{
    /// <summary>
    /// Button
    /// </summary>
    /// 
    enum TypeButton
    {
        Left,
        Right,
        LeftU,
        RightU
    }
    class Button
    {
        private Tile texturePressed;

        public Tile TexturePressed
        {
            get { return texturePressed; }
            set { texturePressed = value; }
        }
        private Tile textureUnPressed;

        public Tile TextureUnPressed
        {
            get { return textureUnPressed; }
            set { textureUnPressed = value; }
        }
        private Vector2 position;
        private TypeButton typeButton;

        public TypeButton TypeButton
        {
            get { return typeButton; }
            set { typeButton = value; }
        }

        public readonly Color Color = Color.Red;

        private bool pressed =false;

        public bool Pressed
        {
            get { return pressed; }
            set { Pressed = value; }
        }

    
        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

     
        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public Button(Level level, Vector2 position,TypeButton type)
        {
            this.level = level;
            this.position = position;
            this.typeButton = type;
            LoadContent();
        }


        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {

            switch (typeButton)
            {

                case TypeButton.Left:
                    {
                        TexturePressed = new Tile(Level.Content.Load<Texture2D>("Sprites/Button/Left"),TileCollision.Impassable);
                      
                        position = new Vector2(TexturePressed.Texture.Width / 2.0f, TexturePressed.Texture.Height / 2.0f);
                        break;
                    }

                case TypeButton.Right:
                    {
                        TexturePressed = new Tile(Level.Content.Load<Texture2D>("Sprites/Button/Right"), TileCollision.Impassable);

                        position = new Vector2(TexturePressed.Texture.Width / 2.0f, TexturePressed.Texture.Height / 2.0f);
                        break;
                    }
                case TypeButton.LeftU:
                    {

                        TextureUnPressed = new Tile(Level.Content.Load<Texture2D>("Sprites/Button/LeftU"), TileCollision.Impassable);
                        position = new Vector2(TextureUnPressed.Texture.Width / 2.0f, TextureUnPressed.Texture.Height / 2.0f);
                        break;
                    }
                case TypeButton.RightU:
                    {
                        TextureUnPressed = new Tile(Level.Content.Load<Texture2D>("Sprites/Button/RightU"), TileCollision.Impassable);
                        position = new Vector2(TextureUnPressed.Texture.Width / 2.0f, TextureUnPressed.Texture.Height / 2.0f);
                        break;
                    }

            }

        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {

            //Texture2D texture;
            //if (pressed)
            //{
            //    texture = texturePressed;
            //}
            //else
            //    texture = textureUnPressed;

            //if (pressed)
            //{
            //    origin = new Vector2(texturePressed.Width / 2.0f, texturePressed.Height / 2.0f);
            //}
            //else
            //{
            //    origin = new Vector2(textureUnPressed.Width / 2.0f, textureUnPressed.Height / 2.0f);
            //}


        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D texture;
            //if (!pressed)
            {
                texture = TexturePressed.Texture;
            }
           // else
          //      texture = TextureUnPressed.Texture;

            spriteBatch.Draw(texture, Position, Color);
        }
    }
}
