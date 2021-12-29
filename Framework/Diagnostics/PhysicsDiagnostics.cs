using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.ECS.Systems;

namespace ScrapBox.Framework.Diagnostics
{
    public static class PhysicsDiagnostics
    {
        public static int FPS { get; internal set; }

        public static SpriteFont Font { get; set; }

        public static void Draw(ScrapVector offset)
        {
            if (Font == null)
                return;

            Vector2 TextOffset = Font.MeasureString("Physics Diagnostics");

            Renderer.RenderText(Font, "Physics Diagnostics", offset, Color.White);
            Renderer.RenderText(Font, $"FPS {FPS}", offset + new ScrapVector(0, TextOffset.Y), Color.White);
            Renderer.RenderText(Font, $"Static bodies: {PhysicsSystem.Static}", offset + new ScrapVector(0, TextOffset.Y * 2), Color.White);
            Renderer.RenderText(Font, $"Dynamic bodies: {PhysicsSystem.Dynamic}", offset + new ScrapVector(0, TextOffset.Y * 3), Color.White);
            Renderer.RenderText(Font, $"Physics system update: {PhysicsSystem.TimeTaken} ms", offset + new ScrapVector(0, TextOffset.Y * 4), Color.White);
        }
    }
}
