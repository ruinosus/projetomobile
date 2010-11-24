using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class CustomImage
    {
        private float timer;
        public CustomImage(Texture2D initialImage, Vector2 position, Vector2 origin, float transitionTime)
        {
            this.Image = initialImage;
            this.TransitionTime = transitionTime;
            this.Color = Color.White;
            this.Rotation = 0;
            this.Position = position;
            this.Origin = origin;
        }

        public float TransitionTime { get; set; }

        public Texture2D Image { get; private set; }

        public Texture2D ToImage { get; private set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position { get; set; }

        public Color Color { get; set; }

        public float Rotation { get; set; }

        public void TransitionTo(Texture2D image)
        {
            if (this.ToImage != null)
            {
                return;
            }
            this.ToImage = image;
            this.timer = 0;
        }

        public void Draw(SpriteBatch batch)
        {
            if (this.ToImage == null)
            {
                batch.Draw(this.Image, this.Position, null, this.Color, this.Rotation, this.Origin, 1f, SpriteEffects.None, 0);
            }
            else
            {
                int alpha = (int)((this.timer / this.TransitionTime) * 255);
                this.DrawImage(batch, this.Image, 255 - alpha);
                this.DrawImage(batch, this.ToImage, alpha);
            }
        }

        public void Update(float elapsed)
    {
        // We must be transitioning
        if (this.ToImage != null)
        {
            this.timer += elapsed;
            if (this.timer == this.TransitionTime)
            {
                this.Image = this.ToImage;
                this.ToImage = null;
            }
        }
    }

        private void DrawImage(SpriteBatch batch, Texture2D texture, int alpha)
        {
            //batch.Draw(texture, this.Position, null, new Color(this.Color, (byte)alpha), this.Rotation, this.Origin, 1f, SpriteEffects.None, 0);
            batch.Draw(texture, this.Position, null, Color, this.Rotation, this.Origin, 1f, SpriteEffects.None, 0);
        }
    }
}
