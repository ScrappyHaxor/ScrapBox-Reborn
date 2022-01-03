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
        internal static Color pixelColor;
        internal static BasicEffect defaultShader;

        public const int MAX_CIRCLE_POINTS = 256;
        public const int MIN_CIRCLE_POINTS = 8;

        public const int MAX_POLYGON_POINTS = 256;

        internal static bool begunRendering;

        internal static void Initialize(SpriteBatch spriteBatch)
        {
            LogService.Log("Renderer2D", "Initialize", "Renderer successfully initialized.", Severity.INFO);
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new Color[] { new Color(255, 255, 255, 255) });
            pixelColor = new Color(255, 255, 255, 255);
            defaultShader = new BasicEffect(batch.GraphicsDevice)
            {
                Texture = pixel,
                TextureEnabled = true
            };
        }

        internal static void BeginRender(Color? color = null, Camera camera = null, Effect shader = null)
        {
            if (batch == null)
            {
                //This is mainly a legacy error just kept for robustness. It should never be raised.
                LogService.Log("Renderer2D", "RenderSprite", "Tried to render without proper initialization", Severity.CRITICAL);
                return;
            }

            Matrix? transformationMatrix = null;
            if (camera != null)
            {
                transformationMatrix = camera.TransformationMatrix;
            }

            if (color != null && (Color)color != pixelColor)
            {
                //Most likely more efficient to use a shader here in the future
                pixel.SetData(new Color[] { (Color)color });
                pixelColor = (Color)color;
            }

            begunRendering = true;
            Batch.Begin(transformMatrix: transformationMatrix, effect: shader, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
        }

        internal static void EndRender()
        {
            if (!begunRendering)
                return;

            begunRendering = false;
            Batch.End();
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
            if (texture == null)
            {
                LogService.Log("Renderer2D", "RenderSprite", "Tried to render null texture.", Severity.WARNING);
                return;
            }
            
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);
            
            //Culling
            if (camera != null)
            {
                Rectangle visibleArea = camera.VisibleArea;
                if (position.X + dimensions.X < visibleArea.X || position.X - dimensions.X > visibleArea.X + visibleArea.Width ||
                    position.Y + dimensions.Y < visibleArea.Y || position.Y - dimensions.Y > visibleArea.Y + visibleArea.Height)
                {
                    return;
                }
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

            BeginRender(null, camera, shader);            
            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin, effects, depth);
            EndRender();
        }
        #endregion

        #region Primitives
        public static void RenderLine(ScrapVector pointA, ScrapVector pointB, Color color, Camera camera = null, Effect shader = null, double thickness = 1)
        {
            double distance = ScrapMath.Distance(pointA, pointB);
            double angle = ScrapMath.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);

            RenderLine(pointA, distance, angle, color, camera, shader, thickness);
        }

        public static void RenderLine(ScrapVector point, double length, double theta, Color color, Camera camera = null, Effect shader = null, double thickness = 1)
        {
            RenderDiagnostics.Calls++;

            ScrapVector origin = new ScrapVector(0, 0.5f);
            ScrapVector scale = new ScrapVector(length, thickness);

            BeginRender(color, camera, shader);
            batch.Draw(pixel, point, null, color, (float)theta, origin, scale, SpriteEffects.None, 0);
            EndRender();
        }

        public static void RenderPolygonOutline(ScrapVector[] verts, Color color, Camera camera = null, Effect shader = null)
        {
            if (verts.Length == 0)
                return;

            if (verts.Length > MAX_POLYGON_POINTS)
            {
                LogService.Log("Renderer", "RenderPolygonOutline", "Polygon point count exceeds renderer limitations.", Severity.ERROR);
                return;
            }    

            for (int i = 0; i < verts.Length; i++)
            {
                ScrapVector start = verts[i];
                ScrapVector end = verts[(i + 1) % verts.Length];

                RenderLine(start, end, color, camera, shader);
            }
        }

        public static void RenderPolygon(ScrapVector[] verts, Color fillColor, Color? borderColor = null, Camera camera = null, Effect shader = null)
        {
            if (borderColor == null)
            {
                borderColor = fillColor;
            }

            //VertexPositionColor[] vertsPosTextures = new VertexPositionColor[verts.Length];
            //for (int i = 0; i < vertsPosTextures.Length; i++)
            //{
            //    VertexPositionColor vertPosTexture = vertsPosTextures[i];
            //    vertPosTexture.Position = verts[i];
            //}

            defaultShader.Texture.SetData(new Color[] { fillColor });

            VertexPositionColor[] vertsPosColors = new VertexPositionColor[4];
            vertsPosColors[0].Position = new Vector3(0, 0, 0);
            vertsPosColors[1].Position = new Vector3(100, 0, 0);
            vertsPosColors[2].Position = new Vector3(0, 100, 0);
            vertsPosColors[3].Position = new Vector3(100, 100, 0);

            short[] indicies = { 0, 2, 1, 1, 2, 3 };

            foreach (EffectPass pass in defaultShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                batch.GraphicsDevice.DrawUserIndexedPrimitives(0, vertsPosColors, 0, vertsPosColors.Length, indicies, 0, indicies.Length / 3);
            }
        }

        public static void RenderBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null, Effect shader = null)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderDiagnostics.Calls++;

            BeginRender(color, camera, shader);
            batch.Draw(pixel, bounds, null, color, rotation, center, SpriteEffects.None, 0);
            EndRender();
        }

        public static void RenderBox(ScrapVector posA, ScrapVector posB, Color color, Camera camera = null, Effect shader = null)
        {
            if (posA.X > posB.X && posA.Y > posB.Y)
            {
                ScrapMath.Swap(ref posA, ref posB);
            }

            BeginRender(color, camera, shader);
            batch.Draw(pixel, new Rectangle((int)posA.X, (int)posA.Y, (int)(posB.X - posA.X), (int)(posB.Y - posA.Y)), color);            
            EndRender();
        }

        public static void RenderOutlineBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null, Effect shader = null)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderDiagnostics.Calls++;

            BeginRender(color, camera, shader);
            batch.Draw(pixel, new Rectangle(bounds.X, bounds.Y - bounds.Height / 2, bounds.Width, 1), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X, bounds.Y + bounds.Height / 2, bounds.Width, 1), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X - bounds.Width / 2, bounds.Y, 1, bounds.Height), null, color, rotation, center, SpriteEffects.None, 0);
            batch.Draw(pixel, new Rectangle(bounds.X + bounds.Width / 2, bounds.Y, 1, bounds.Height), null, color, rotation, center, SpriteEffects.None, 0);
            EndRender();
        }

        public static void RenderCircle(ScrapVector position, double radius, int pointCount, Color color, Camera camera = null, Effect shader = null, int thickness = 1)
        {
            pointCount = (int)ScrapMath.Clamp(pointCount, MIN_CIRCLE_POINTS, MAX_CIRCLE_POINTS);

            double theta = ScrapMath.PI2 / pointCount;

            double sin = ScrapMath.Sin(theta);
            double cos = ScrapMath.Cos(theta);

            ScrapVector current = new ScrapVector(radius, 0);
            for (int i = 0; i < pointCount; i++)
            {
                ScrapVector nextPoint = new ScrapVector(
                    current.X * cos - current.Y * sin,
                    current.X * sin + current.Y * cos);

                RenderLine(current + position, nextPoint + position, color, camera, shader, thickness);

                current = nextPoint;
            }
        }

        public static void RenderGrid(ScrapVector pointA, ScrapVector pointB, ScrapVector tileSize, Color color, Camera camera = null, Effect shader = null, double thickness = 1)
        {
            RenderDiagnostics.Calls++;

            for (double x = pointA.X; x < pointB.X; x += tileSize.X)
            {
                for (double y = pointA.Y; y < pointB.Y; y += tileSize.Y)
                {
                    RenderLine(new ScrapVector(x, pointA.Y), new ScrapVector(x, pointB.Y), color, camera, shader, thickness);
                    RenderLine(new ScrapVector(pointA.X, y), new ScrapVector(pointB.X, y), color, camera, shader, thickness);
                }
            }

            RenderLine(new ScrapVector(pointB.X, pointA.Y), new ScrapVector(pointB.X, pointB.Y), color, camera, shader, thickness);
            RenderLine(new ScrapVector(pointA.X, pointB.Y), new ScrapVector(pointB.X, pointB.Y), color, camera, shader, thickness);
        }
        #endregion

        #region Text
        public static ScrapVector MeasureText(SpriteFont font, string text)
        {
            Vector2 mono = font.MeasureString(text);
            return new ScrapVector(mono.X, mono.Y);
        }

        public static void RenderText(SpriteFont font, string label, ScrapVector position, Color textColor, Camera camera = null, Effect shader = null)
        {
            RenderDiagnostics.Calls++;

            BeginRender(textColor, camera, shader);            
            batch.DrawString(font, label, position, textColor);
            EndRender();
        }

        public static void RenderCenteredText(SpriteFont font, string label, ScrapVector position, Color textColor, Camera camera = null, Effect shader = null)
        {
            ScrapVector size = (ScrapVector)font.MeasureString(label);
            RenderText(font, label, position - size / 2, textColor, camera, shader);
        }

        #endregion
    }
}
