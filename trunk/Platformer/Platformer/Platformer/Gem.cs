#region File Description
//-----------------------------------------------------------------------------
// Gem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    public enum TipoGem
    {
        Normal,
        PowerUp,
        Live
    }

    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class Gem
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect collectedSound;
        private TipoGem tipoGem;
        public TipoGem TipoGem
        {
            get { return tipoGem; }
            set { tipoGem = value; }
        }
        public readonly int PointValue;
        public readonly Color Color;



        // The gem is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

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
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public Gem(Level level, Vector2 position, TipoGem tipoGem)
        {
            this.level = level;
            this.basePosition = position;
            this.tipoGem = tipoGem;
            switch (tipoGem)
            {
                case TipoGem.Normal:
                    {
                        PointValue = 30;
                        Color = Color.Yellow;
                        break;
                    }
                case TipoGem.PowerUp:
                    {
                        PointValue = 100;
                        Color = Color.White;
                        break;
                    }
                case TipoGem.Live:
                    {
                        PointValue = 0;
                        Color = Color.Red;
                        break;
                    }
                default:
                    break;
            }


            LoadContent();
        }

        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {

            switch (tipoGem)
            {
                case TipoGem.Normal:
                    {
                        texture = Level.Content.Load<Texture2D>("Sprites/Gem");
                        break;
                    }
                case TipoGem.PowerUp:
                    {
                        texture = Level.Content.Load<Texture2D>("Sprites/star");
                        break;
                    }
                case TipoGem.Live:
                    {
                        texture = Level.Content.Load<Texture2D>("Sprites/Gem");
                        break;
                    }
                default:
                    break;
            }


            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            collectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        public void OnCollected(Player collectedBy)
        {



            switch (tipoGem)
            {
                case TipoGem.Normal:
                    {
                        collectedSound.Play();
                        break;
                    }
                case TipoGem.PowerUp:
                    {
                        collectedBy.PowerUp();
                        break;
                    }
                case TipoGem.Live:
                    {
                        collectedSound.Play();
                        collectedBy.LiveUp();
                        break;
                    }
                default:
                    break;
            }


        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
