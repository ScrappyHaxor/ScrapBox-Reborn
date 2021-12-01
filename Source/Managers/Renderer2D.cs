using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.ECS.Components;
using ScrapBox.SMath;
using ScrapBox.Scene;

using System;

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
            pixel.SetData(new Color[] { new Color(255, 255, 255) });
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, Camera camera, float depth = 0)
        {
            RenderSprite(texture, position, new ScrapVector(texture.Width, texture.Height), camera, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, Camera camera, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, camera, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Camera camera, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, camera, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Camera camera,
                float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, Color.White, camera, depth);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
                Camera camera, float depth = 0)
        {
            RenderSprite(texture, position, dimensions, 0, null, tintColor, SpriteEffects.None, camera, depth);
        }

        public static void RenderSprite(Sprite2D sprite, Camera camera)
        {
            if (camera == null)
                return;

            RenderSprite(sprite.Texture, sprite.Transform.Position, sprite.Transform.Dimensions, sprite.Transform.Rotation, sprite.SourceRectangle, 
                    sprite.TintColor, 
                    sprite.Effects, camera, sprite.Depth);
        }

        public static void RenderSpriteTransformed(Sprite2D Sprite, Matrix transform)
        {
            batch.Begin(transformMatrix: transform);
            batch.Draw(Sprite.Texture, Vector2.Zero, Color.White);
            batch.End();
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
                SpriteEffects effects, Camera camera, float depth = 0)
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
            
            //Culling
            Rectangle visibleArea = camera.VisibleArea;
            if (position.X + dimensions.X < visibleArea.X || position.X - dimensions.X > visibleArea.X + visibleArea.Width ||
                position.Y + dimensions.Y < visibleArea.Y || position.Y - dimensions.Y > visibleArea.Y + visibleArea.Height)
            {
                return;
            }

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
            
            batch.Begin(transformMatrix: camera.TransformationMatrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Deferred);
            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin.ToMono(), effects, depth);
            batch.End();
        }

        public static void RenderPrimitiveBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);

            batch.Begin();//transformMatrix: camera.TransformationMatrix);

            batch.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y), null, color, rotation, center.ToMono(), 
                    SpriteEffects.None, 1f);
            batch.End();
        }

        public static void RenderText(SpriteFont font, string label, ScrapVector position, Color textColor)
        {
            batch.Begin();
            batch.DrawString(font, label, position.ToMono(), textColor);
            batch.End();
        }
    }
}
