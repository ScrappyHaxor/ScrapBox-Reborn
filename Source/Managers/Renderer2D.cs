using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.ECS.Components;
using ScrapBox.SMath;

namespace ScrapBox.Managers
{
    public static class Renderer2D
    {
        internal static SpriteBatch batch;
        internal static Texture2D pixel;

        internal static void Initialize(SpriteBatch spriteBatch)
        {
            LogManager.Log(new LogMessage("Renderer2D", "Initialized.", LogMessage.Severity.VERBOSE));
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new Color[] { Color.White });
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, float depth = 0)
        {
            RenderSprite(texture, position, new ScrapVector(texture.Width, texture.Height), depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, Color.White, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
                float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, tintColor, SpriteEffects.None, depth);
        }

        public static void RenderSprite(Sprite2D sprite)
        {
            RenderSprite(sprite.Texture, sprite.Transform.Position, sprite.Transform.Dimensions, sprite.Transform.Rotation, sprite.SourceRectangle, 
                    sprite.TintColor, 
                    sprite.Effects, sprite.Depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
                SpriteEffects effects, float depth = 0)
        {
            if (batch == null)
            {
                LogManager.Log(new LogMessage("Renderer2D", "Tried to render without initialization. Make sure your game class implements ScrapBox/Root/ScrapApp", 
                            LogMessage.Severity.CRITICAL));
                return;
            }

            if (texture == null)
            {
                LogManager.Log(new LogMessage("Renderer2D", "Tried to render null texture.", LogMessage.Severity.WARNING));
                return;
            }
            
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            ScrapVector origin;
            if (sourceRectangle == null)
            {
                origin = new ScrapVector(dimensions.X / 2 - (dimensions.X - texture.Width) / 2, dimensions.Y / 2 - (dimensions.Y - texture.Height) / 2);
            }
            else
            {
                Rectangle rect = (Rectangle)sourceRectangle;
                origin = new ScrapVector(dimensions.X / 2 - (dimensions.X - rect.Width) / 2, dimensions.Y /2 - (dimensions.Y - rect.Height) / 2);
            }
            
            batch.Begin();
            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin.ToMono(), effects, depth);
            batch.End();
        }

        public static void RenderPrimitiveHitbox(ScrapVector position, ScrapVector dimensions, float rotation, Color color)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
           
            //batch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y), null, color, rotation, center, SpriteEffects.None, 1f);
            
            batch.Begin();
            batch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y-(int)dimensions.Y/2, (int)dimensions.X, 1), 
                    null, color, rotation, center.ToMono(), SpriteEffects.None, 1f);
            batch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y+(int)dimensions.Y/2, (int)dimensions.X, 1), 
                    null, color, rotation, center.ToMono(), SpriteEffects.None, 1f);
            batch.Draw(pixel, new Rectangle((int)position.X-(int)dimensions.X/2, (int)position.Y, 1, (int)dimensions.Y), 
                    null, color, rotation, center.ToMono(), SpriteEffects.None, 1f);
            batch.Draw(pixel, new Rectangle((int)position.X+(int)dimensions.X/2, (int)position.Y, 1, (int)dimensions.Y), 
                    null, color, rotation, center.ToMono(), SpriteEffects.None, 1f);
            batch.End();
        }
    }
}
