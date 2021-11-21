using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.ECS.Components;

namespace ScrapBox.Managers
{
    public static class Renderer2D
    {
        internal static SpriteBatch batch;

        internal static void Initialize(SpriteBatch spriteBatch)
        {
            LogManager.Log(new LogMessage("Renderer2D", "Initialized.", LogMessage.Severity.VERBOSE));
            batch = spriteBatch;
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, float depth = 0)
        {
            RenderSprite(texture, position, Vector2.One, depth);
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, Vector2 dimensions, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, depth);
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, Vector2 dimensions, float rotation, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, depth);
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, Vector2 dimensions, float rotation, Rectangle? sourceRectangle, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, Color.White, depth);
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, Vector2 dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, tintColor, SpriteEffects.None, depth);
        }

        public static void RenderSprite(Sprite2D sprite)
        {
            RenderSprite(sprite.Texture, sprite.Transform.Position, sprite.Transform.Dimensions, sprite.Transform.Rotation, sprite.SourceRectangle, sprite.TintColor, 
                    sprite.Effects, sprite.Depth);
        }

        public static void RenderSprite(Texture2D texture, Vector2 position, Vector2 dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
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

            Vector2 origin;
            if (sourceRectangle == null)
            {
                origin = new Vector2(dimensions.X / 2 - (dimensions.X - texture.Width) / 2, dimensions.Y / 2 - (dimensions.Y - texture.Height) / 2);
            }
            else
            {
                Rectangle rect = (Rectangle)sourceRectangle;
                origin = new Vector2(dimensions.X / 2 - (dimensions.X - rect.Width) / 2, dimensions.Y /2 - (dimensions.Y - rect.Height) / 2);
            }

            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin, effects, depth);
        }
    }
}
