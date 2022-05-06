using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Diagnostics;
using ScrapBox.Framework.Generic;
using System;

namespace ScrapBox.Framework.Services
{
    public static class Renderer
    {
        public static SpriteBatch Batch { get { return batch; } }
        public static Texture2D Pixel { get { return pixel; } }

        public static Effect PostProcessing { get; set; }

        public static Color ClearColor { get; set; }

        internal static SpriteBatch batch;
        internal static Texture2D pixel;
        internal static Color pixelColor;
        internal static BasicEffect defaultShader;

        internal static RenderTarget2D sceneTarget;

        public const int MAX_CIRCLE_POINTS = 256;
        public const int MIN_CIRCLE_POINTS = 8;

        public const int MAX_POLYGON_POINTS = 256;

        internal static bool begunRendering;

        internal static void Initialize(SpriteBatch spriteBatch)
        {
            
            batch = spriteBatch;
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new Color[] { new Color(255, 255, 255, 255) });
            pixelColor = new Color(255, 255, 255, 255);

            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            batch.GraphicsDevice.RasterizerState = state;

            defaultShader = new BasicEffect(batch.GraphicsDevice)
            {
                Texture = pixel,
                TextureEnabled = true
            };

            sceneTarget = new RenderTarget2D(Batch.GraphicsDevice,
                batch.GraphicsDevice.PresentationParameters.BackBufferWidth, batch.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                batch.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            LogService.Log("Renderer2D", "Initialize", "Renderer successfully initialized.", Severity.INFO);
        }

        internal static void BeginSceneRender()
        {
            Batch.GraphicsDevice.SetRenderTarget(sceneTarget);
            Batch.GraphicsDevice.Clear(ClearColor);
            Batch.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
        }

        public static void BeginRenderToTarget(RenderTarget2D renderTarget)
        {
            Batch.GraphicsDevice.SetRenderTarget(renderTarget);
            Batch.GraphicsDevice.Clear(Color.Transparent);
        }

        public static void RenderTargetToScene(RenderTarget2D renderTarget)
        {
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(renderTarget, new Rectangle(0, 0, batch.GraphicsDevice.Viewport.Width, batch.GraphicsDevice.Viewport.Height), Color.White);
            batch.End();
        }

        internal static void BeginRender(Color? color = null, Camera camera = null, Effect shader = null, SamplerState sampler = null)
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
            Batch.Begin(transformMatrix: transformationMatrix, effect: shader, samplerState: sampler, sortMode: SpriteSortMode.Immediate);
        }

        internal static void EndRender()
        {
            if (!begunRendering)
                return;

            begunRendering = false;
            Batch.End();
        }

        public static void EndRenderToTarget()
        {
            Batch.GraphicsDevice.SetRenderTarget(sceneTarget);
        }

        internal static void EndSceneRender()
        {
            Batch.GraphicsDevice.SetRenderTarget(null);

            Batch.GraphicsDevice.Clear(ClearColor);
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, effect: PostProcessing);
            batch.Draw(sceneTarget, new Rectangle(0, 0, batch.GraphicsDevice.Viewport.Width, batch.GraphicsDevice.Viewport.Height), Color.White);
            batch.End();
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
                if (!camera.InView(position, dimensions))
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

            BeginRender(null, camera, shader, SamplerState.PointClamp);            
            batch.Draw(texture, bounds, sourceRectangle, tintColor, rotation, origin, effects, depth);
            EndRender();
        }
        #endregion

        #region RenderTileable

        public static void RenderTileable(Texture2D texture, ScrapVector position, ScrapVector dimensions, Camera camera = null, Effect shader = null)
        {
            RenderTileable(texture, position, dimensions, camera, shader);
        }

        public static void RenderTileable(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, 
            Camera camera = null, Effect shader = null)
        {
            RenderTileable(texture, position, dimensions, rotation, Color.White, camera, shader);
        }

        public static void RenderTileable(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Color tintColor, 
            Camera camera = null, Effect shader = null)
        {
            RenderTileable(texture, position, dimensions, rotation, tintColor, SpriteEffects.None, camera, shader);
        }


        public static void RenderTileable(Sprite2D sprite, Camera camera = null)
        {
            RenderTileable(sprite.Texture, sprite.Position, sprite.Transform.Dimensions, (float)sprite.Rotation,
                sprite.TintColor, sprite.Effects, camera, sprite.Shader);
        }

        public static void RenderTileable(Texture2D texture, ScrapVector position, ScrapVector dimensions, float rotation, Color tintColor,
                SpriteEffects effects, Camera camera, Effect shader = null, float depth = 0)
        {
            if (texture == null)
            {
                LogService.Log("Renderer2D", "RenderTileable", "Tried to render null texture.", Severity.WARNING);
                return;
            }

            //Culling
            if (camera != null)
            {
                if (!camera.InView(position, dimensions))
                    return;
            }

            RenderDiagnostics.Calls++;

            //TODO: Fix this mess
            BeginRender(null, camera, shader, SamplerState.LinearClamp);
            for (int x = (int)(position.X - dimensions.X / 2); x < (int)(position.X + dimensions.X / 2); x += texture.Width)
            {
                for (int y = (int)(position.Y - dimensions.Y / 2); y < (int)(position.Y + dimensions.Y / 2); y += texture.Height)
                {
                    Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                    if (x + texture.Width > position.X + dimensions.X / 2)
                        sourceRectangle.Width = texture.Width - (int)(x + texture.Width - (position.X + dimensions.X / 2));

                    if (y + texture.Height > position.Y + dimensions.Y / 2)
                        sourceRectangle.Height = texture.Height - (int)(y + texture.Height - (position.Y + dimensions.Y / 2));

                    batch.Draw(texture, new Rectangle(x, y, sourceRectangle.Width, sourceRectangle.Height), sourceRectangle, tintColor, rotation, ScrapVector.Zero, effects, depth);
                }
            }
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

            BeginRender(color, camera, shader, SamplerState.PointClamp);
            batch.Draw(pixel, point, null, color, (float)theta, origin, scale, SpriteEffects.None, 0);
            EndRender();
        }

        public static void RenderPolygonOutline(ScrapVector[] verts, Color color, Camera camera = null, Effect shader = null, double lineThickness = 1)
        {
            if (verts == null || verts.Length == 0)
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

                RenderLine(start, end, color, camera, shader, lineThickness);
            }
        }

        public static void RenderPolygon(ScrapVector[] verticies, int[] indicies, Color fillColor, Camera camera = null, Effect shader = null)
        {
            if (shader == null)
            {
                shader = new BasicEffect(batch.GraphicsDevice);
            }

            BasicEffect basic = new BasicEffect(batch.GraphicsDevice);
            basic.World = Matrix.CreateTranslation(new Vector3(-(float)camera.Position.X, (float)camera.Position.Y, 0)) * Matrix.CreateScale((float)camera.Zoom); //camera.TransformationMatrix;
            basic.View = camera.ViewMatrix;
            basic.Projection = camera.ProjectionMatrix;
            basic.VertexColorEnabled = true;

            VertexPositionColor[] verticiesColors = new VertexPositionColor[verticies.Length];
            for (int i = 0; i < verticies.Length; i++)
            {
                ScrapVector vertex = verticies[i];
                verticiesColors[i] = new VertexPositionColor(new Vector3((float)vertex.X, -(float)vertex.Y, 0), fillColor);
            }

            foreach (EffectPass pass in basic.CurrentTechnique.Passes)
            {
                pass.Apply();
                batch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, verticiesColors, 0, verticiesColors.Length, indicies, 0, indicies.Length / 3);
            }
        }

        public static void RenderPolygonWireframe(ScrapVector[] verticies, int[] indicies, Color lineColor, Camera camera = null, Effect shader = null, double lineThickness = 1)
        {
            for (int i = 0; i < indicies.Length; i += 3)
            {
                int first = indicies[i];
                int second = indicies[i + 1];
                int third = indicies[i + 2];

                ScrapVector firstVertex = verticies[first];
                ScrapVector secondVertex = verticies[second];
                ScrapVector thirdVertex = verticies[third];

                RenderLine(firstVertex, secondVertex, lineColor, camera, shader, lineThickness);
                RenderLine(secondVertex, thirdVertex, lineColor, camera, shader, lineThickness);
                RenderLine(thirdVertex, firstVertex, lineColor, camera, shader, lineThickness);
            }
        }

        [Obsolete("Use RenderPolygon instead with a polygon from ScrapBox.Framework.Shapes and TriangulationService from ScrapBox.Framework.Services")]
        public static void RenderBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null, Effect shader = null)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderDiagnostics.Calls++;

            BeginRender(color, camera, shader, SamplerState.PointClamp);
            batch.Draw(pixel, bounds, null, color, rotation, center, SpriteEffects.None, 0);
            EndRender();
        }

        [Obsolete("Use RenderPolygonOutline instead with a polygon from ScrapBox.Framework.Shapes and TriangulationService from ScrapBox.Framework.Services")]
        public static void RenderOutlineBox(ScrapVector position, ScrapVector dimensions, float rotation, Color color, Camera camera = null, Effect shader = null, double lineThickness = 1)
        {
            ScrapVector center = new ScrapVector(dimensions.X / 2 - (dimensions.X - pixel.Width) / 2, dimensions.Y / 2 - (dimensions.Y - pixel.Height) / 2);
            Rectangle bounds = new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);

            RenderLine(new ScrapVector(bounds.X - bounds.Width / 2, bounds.Y - bounds.Height / 2), new ScrapVector(bounds.X - bounds.Width / 2, bounds.Y + bounds.Height / 2), color, camera, shader, lineThickness);
            RenderLine(new ScrapVector(bounds.X + bounds.Width / 2, bounds.Y - bounds.Height / 2), new ScrapVector(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), color, camera, shader, lineThickness);
            RenderLine(new ScrapVector(bounds.X - bounds.Width / 2, bounds.Y - bounds.Height / 2), new ScrapVector(bounds.X + bounds.Width / 2, bounds.Y - bounds.Height / 2), color, camera, shader, lineThickness);
            RenderLine(new ScrapVector(bounds.X - bounds.Width / 2, bounds.Y + bounds.Height / 2), new ScrapVector(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), color, camera, shader, lineThickness);
        }

        [Obsolete("Use RenderPolygon instead with an ellipse from ScrapBox.Framework.Shapes and TriangulationService from ScrapBox.Framework.Services")]
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
            if (font == null)
            {
                LogService.Log("Renderer2D", "RenderText", "Font provided was null.", Severity.ERROR);
                return;
            }

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
