#region File Description
//-----------------------------------------------------------------------------
// Player.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Platformer
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    class Player
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        private Animation dieAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;
        private int lives;
        private const int totalInitialLives = 3;
        bool pressed;
        private int score;
        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        // Sounds
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;


        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        private const float MaxPowerUpTime = 19.0f;
        private float powerUpTime;
        public bool IsPoweredUp
        {
            get { return powerUpTime > 0.0f; }
            set { IsPoweredUp = value; }
        }
        private readonly Color[] poweredUpColors = {
                               Color.Black,
                               Color.White,
                               Color.Black,
                               Color.White,
                                               };
        private SoundEffect powerUpSound;
        private SoundEffect liveUpSound;


        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 700f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;
        private int numberOfJumps = 0;
        // Input configuration
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.5f;
        private const Buttons JumpButton = Buttons.A;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;
        float fingerX = 0, fingerY = 0;
        float fingerXOld = 0, fingerYOld = 0;
        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Rectangle FingerRectangle
        {
            get
            {
                int left = (int)Math.Round(fingerX - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(fingerX - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, 100000);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Player(Level level, Vector2 position, int lives)
        {
            this.lives = lives;
            this.level = level;

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
            celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);
            powerUpSound = Level.Content.Load<SoundEffect>("Sounds/PowerUp");
            // liveUpSound = Level.Content.Load<SoundEffect>("Sounds/PowerUp");

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            MediaPlayer.Stop();
            Velocity = Vector2.Zero;
            powerUpTime = 0.0f;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        public void ResetInitial(Vector2 position)
        {
            Reset(position);
            lives = totalInitialLives;
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            TouchCollection touchState,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            velocity.X = 0;
            movement = 0;
            GetInput(keyboardState, gamePadState, touchState, accelState, orientation);
            if (!BoundingRectangle.Intersects(FingerRectangle) )
            {
                ApplyPhysics(gameTime);

                if (IsAlive && IsOnGround)
                {

                    if (Math.Abs(Velocity.X) - 0.2f > 0)
                    {
                        sprite.PlayAnimation(runAnimation);
                    }
                    else
                    {
                        sprite.PlayAnimation(idleAnimation);
                    }
                }
            }
            else if (isOnGround)
                sprite.PlayAnimation(idleAnimation);
            else
            {
                ApplyPhysics(gameTime);
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
            if (isOnGround)
                numberOfJumps = 0;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            TouchCollection touchState,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            //override digital if touch input is found
            // Process touch locations.
            bool touchJump = false;

            //#if FakeAccelerometer
            //            touchJump = Accelerometer.GetState().IsJumping;
            //#endif
            //get the state of the touch panel
            //TouchCollection curTouches = TouchPanel.GetState();

            movement = 0.0f;
            // Process touch locations
            pressed = false;
            //esquerda = false;

            foreach (TouchLocation location in touchState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                        {
                            //fingerXOld = fingerX;
                            //fingerYOld = fingerY;

                            fingerX = (float)Math.Round(location.Position.Y); ;
                            fingerY = (float)Math.Round(location.Position.X);
                            if (fingerX > 0)
                                pressed = true;
                            if (fingerY < (BoundingRectangle.X))
                                touchJump = true;
                            break;
                        }
                    case TouchLocationState.Released:
                        //Don't care about released state in this demo
                        break;
                    case TouchLocationState.Moved:
                        {
                            //fingerXOld = fingerX;
                            //fingerYOld = fingerY;

                            fingerX = (float)Math.Round(location.Position.X); ;
                            fingerY = (float)Math.Round(location.Position.Y);
                            if (fingerX > 0)
                                pressed = true;
                            //if (!(fingerY > (BoundingRectangle.X - localBounds.Width)))
                           // if (fingerY < (BoundingRectangle.TopY))
                            float pY = Position.Y;
                            float pX = Position.X;
                            float lY = localBounds.Y;
                            float lX = localBounds.X;
                            if ((fingerY+30) < (BoundingRectangle.Y) )
                          //  (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;
                                touchJump = true;                          
                            break;
                        }
                }


            }

            // Ignore small movements to prevent running in place.
            //  if (Math.Abs(movement) < 0.5f)
            movement = 0.0f;

            //if (fingerX != 0)
            //{
            //    if (fingerX >= Position.X)
            //    {
            //        //esquerda = false;
            //        movement = 1.0f;
            //    }
            //    else
            //    {
            //        //esquerda = true;
            //        movement = -1.0f;
            //    }
            //}
            //if (pressed)
            //{
            //    if (fingerX != 0)
            //    {
            //        if (fingerX >= Position.X)
            //        {

            //            movement = 1.0f;
            //        }
            //        else
            //        {

            //            movement = -1.0f;
            //        }
            //    }
            //}
            //else
            //{
            //    if (fingerX != 0)
            //    {
            //        if (fingerX >= Position.X)
            //        {

            //            movement = 1.0f;
            //        }
            //        else
            //        {

            //            movement = -1.0f;
            //        }
            //    }
            //}

            if (fingerX != 0)
            {
                if (fingerX >= Position.X)
                {

                    movement = 1.0f;
                }
                else
                {

                    movement = -1.0f;
                }
            }
            // Check if the player wants to jump.
            isJumping = touchJump;
        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            //   velocity.X = fingerX;

            // if (fingerX < 0)
            //   fingerX *= -1;
            velocity.X = 0;
            // velocity.X = movement * fingerX;
            //if (esquerda)
            //    movement = -1.0f;
            velocity.X = movement * MoveAcceleration;
            // velocity.X += fingerX * movement;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            //   if ((float)Math.Round(fingerX) != (float)Math.Round(position.X))
            {
                Position += velocity * elapsed;

                
            }
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
            // If the player is now colliding with the level, separate them.
             HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    //gives players more control over the top of the jump
                    velocityY =
                        JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump and has double jumps
                    // Irá ter duplo pulo quando tiver com a reliquia magica.
                    if (velocityY > -MaxFallSpeed * 0.5f && !wasJumping && numberOfJumps < 1 && IsPoweredUp)
                    {
                        velocityY =
                            JumpLaunchVelocity * (0.5f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                        jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        numberOfJumps++;
                    }
                    else
                    {
                        jumpTime = 0.0f;
                    }
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            //For each potentially colliding movable tile.  
            foreach (var movableTile in level.movableTiles)
            {
                // Reset flag to search for movable tile collision.  
                movableTile.PlayerIsOn = false;

                //check to see if player is on tile.  
                if ((BoundingRectangle.Bottom == movableTile.BoundingRectangle.Top + 1) &&
                    (BoundingRectangle.Left >= movableTile.BoundingRectangle.Left - (BoundingRectangle.Width / 2) &&
                    BoundingRectangle.Right <= movableTile.BoundingRectangle.Right + (BoundingRectangle.Width / 2)))
                {
                    movableTile.PlayerIsOn = true;
                }

                bounds = HandleCollision(bounds, movableTile.Collision, movableTile.BoundingRectangle);
            }

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        private Rectangle HandleCollision(Rectangle bounds, TileCollision collision, Rectangle tileBounds)
        {
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                // Resolve the collision along the shallow axis.  
                if (absDepthY < absDepthX || collision == TileCollision.Platform)
                {
                    // If we crossed the top of a tile, we are on the ground.  
                    if (previousBottom <= tileBounds.Top)
                        isOnGround = true;

                    // Ignore platforms, unless we are on the ground.  
                    if (collision == TileCollision.Impassable || IsOnGround)
                    {
                        // Resolve the collision along the Y axis.  
                        Position = new Vector2(Position.X, Position.Y + depth.Y);

                        // Perform further collisions with the new bounds.  
                        bounds = BoundingRectangle;
                    }
                }
                else if (collision == TileCollision.Impassable) // Ignore platforms.  
                {
                    // Resolve the collision along the X axis.  
                    Position = new Vector2(Position.X + depth.X, Position.Y);

                    // Perform further collisions with the new bounds.  
                    bounds = BoundingRectangle;
                }
            }
            return bounds;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            isAlive = false;
            lives--;
            if (killedBy != null)
                killedSound.Play();
            else
                fallSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Calculate a tint color based on power up state.
            Color color;
            if (IsPoweredUp)
            {
                float t = ((float)gameTime.TotalGameTime.TotalSeconds + powerUpTime / MaxPowerUpTime) * 20.0f;
                int colorIndex = (int)t % poweredUpColors.Length;
                color = poweredUpColors[colorIndex];
            }
            else
            {
                color = Color.White;
            }


            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);//, color);
        }


        public void PowerUp()
        {
            powerUpTime = MaxPowerUpTime;
            powerUpSound.Play();
            //MediaPlayer.Play(powerUpSound);
        }

        public void LiveUp()
        {
            Lives++;
            // powerUpSound.Play();
        }
    }
}
