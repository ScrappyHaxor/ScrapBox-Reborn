using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Diagnostics;

namespace ScrapBox.Framework.Services
{
    public static class Renderer
    {
        public static SpriteBatch Batch { get { return batch; } }
        public static Texture2D Pixel { get { return pixel; } }

        internal static SpriteBatch batch;
        internal static Texture2D pixel;

        public const int MAX_CIRCLE_POINTS = 256;
        public const int MIN_CIRCLE_POINTS = 8;

        internal static void Initialize(SpriteBatch spriteBatch)
        {
            LogService.Log("Renderer2D", "Initialize", "Renderer successfully initialized.", Severity.INFO);
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new Color[] { new Color(255, 255, 255, 255) });
        }

        #region RenderSprite
        public static void RenderSprite(Texture2D texture, ScrapVector position, Camera camera = null, Effect shader = null)
        {
            RenderSprite(texture, position, new ScrapVector(texture.Width, texture.Height), camera, shader);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, Camera camera = null, Effect shader = null)
        {
            RenderSprite(texture, position, dimensions, 0, camera, shader);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Camera camera = null, Effect shader = null)
        {
            RenderSprite(texture, position, dimensions, rotation, null, Color.White, SpriteEffects.None, camera, shader, 0);
        }

        public static void RenderSprite(Sprite2D sprite, Camera camera = null)
        {
            RenderSprite(sprite.Texture, sprite.Position, sprite.Transform.Dimensions, (float)sprite.Rotation, 
                sprite.SourceRectangle, sprite.TintColor, sprite.Effects, camera, sprite.Shader);
        }

        public static void RenderSprite(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Rectangle? sourceRectangle, Color tintColor, 
                SpriteEffects effects, Camera camera, Effect shader = null, float depth = 0)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderSprite", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            if (texture == null)
            {
                LogService.Log("Renderer2D", "RenderSprite", "Tried to render null texture.", Severity.WARNING);
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

            RenderDiagnostics.Calls++;

            if (shader == null)
            {
                batch.Begin(transformMatrix: camera.TransformationMatrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
            }
            else
            {
                batch.Begin(transformMatrix: camera.TransformationMatrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate, effect: shader);
            }
            
            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin, effects, depth);
            batch.End();
        }
        #endregion

        #region Primitives
        public static void RenderLine(ScrapVector pointA, ScrapVector pointB, Color color, Camera camera = null, double thickness = 1)
        {
            double distance = ScrapMath.Distance(pointA, pointB);
            double angle = ScrapMath.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);

            RenderLine(pointA, distance, angle, color, camera, thickness);
        }

        public static void RenderLine(ScrapVector point, double length, double theta, Color color, Camera camera = null, double thickness = 1)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "DrawLine", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            RenderDiagnostics.Calls++;

            ScrapVector origin = new ScrapVector(0, 0.5f);
            ScrapVector scale = new ScrapVector(length, thickness);

            if (camera == null)
            {
                batch.Begin();
            }
            else
            {
                batch.Begin(transformMatrix: camera.TransformationMatrix);
            }

            batch.Draw(pixel, point, null, color, (float)theta, origin, scale, SpriteEffects.None, 0);
            batch.End();
        }

        public static void RenderBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderPrimitiveBox", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderDiagnostics.Calls++;

            if (camera == null)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            }
            else
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: camera.TransformationMatrix);
            }
            
            batch.Draw(pixel, bounds, null, color, rotation, center, SpriteEffects.None, 0);
            batch.End();
        }

        public static void RenderOutlineBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderPrimitiveBox", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderDiagnostics.Calls++;

            if (camera == null)
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            }
            else
            {
                batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, transformMatrix: camera.TransformationMatrix);
            }

            batch.Draw(pixel, new Rectangle(bounds.X, bounds.Y - bounds.Height / 2, bounds.Width, 1), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X, bounds.Y + bounds.Height / 2, bounds.Width, 1), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X - bounds.Width / 2, bounds.Y, 1, bounds.Height), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X + bounds.Width / 2, bounds.Y, 1, bounds.Height), null, color, rotation, center, SpriteEffects.None, 0);

            batch.End();
        }

        public static void RenderCircle(ScrapVector position, double radius, int pointCount, Color color, Camera camera = null, int thickness = 1)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderCircle", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            pointCount = (int)ScrapMath.Clamp(pointCount, MIN_CIRCLE_POINTS, MAX_CIRCLE_POINTS);

            double theta = ScrapMath.PI2 / pointCount;

            double sin = ScrapMath.Sin(theta);
            double cos = ScrapMath.Cos(theta);

            ScrapVector start = new ScrapVector(radius, 0);
            for (int i = 0; i < pointCount; i++)
            {
                ScrapVector nextPoint = new ScrapVector(
                    start.X * cos - start.Y * sin,
                    start.X * sin + start.Y * cos);

                RenderLine(start + position, nextPoint + position, color, camera, thickness);

                start = nextPoint;
            }
        }

        public static void RenderGrid(ScrapVector pointA, ScrapVector pointB, ScrapVector tileSize, Color color, Camera camera = null, double thickness = 1)
        {
            RenderDiagnostics.Calls++;

            for (double x = pointA.X; x < pointB.X; x += tileSize.X)
            {
                for (double y = pointA.Y; y < pointB.Y; y += tileSize.Y)
                {
                    RenderLine(new ScrapVector(x, pointA.Y), new ScrapVector(x, pointB.Y), color, camera, thickness);
                    RenderLine(new ScrapVector(pointA.X, y), new ScrapVector(pointB.X, y), color, camera, thickness);
                }
            }

            RenderLine(new ScrapVector(pointB.X, pointA.Y), new ScrapVector(pointB.X, pointB.Y), color, camera, thickness);
            RenderLine(new ScrapVector(pointA.X, pointB.Y), new ScrapVector(pointB.X, pointB.Y), color, camera, thickness);
        }
        #endregion

        #region Text
        public static ScrapVector MeasureText(SpriteFont font, string text)
        {
            Vector2 mono = font.MeasureString(text);
            return new ScrapVector(mono.X, mono.Y);
        }

        public static void RenderText(SpriteFont font, string label, ScrapVector position, Color textColor, Camera camera = null)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderText", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            RenderDiagnostics.Calls++;

            if (camera == null)
            {
                batch.Begin();
            }
            else
            {
                batch.Begin(transformMatrix: camera.TransformationMatrix);
            }
            
            batch.DrawString(font, label, position, textColor);
            batch.End();
        }
        #endregion
    }
}
