using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using Microsoft.Xna.Framework;
using ScrapBox.Framework.ECS.Systems;

namespace ScrapBox.Framework.Diagnostics
{
    public static class RenderDiagnostics
    {
        public static int Calls { get; internal set; }
        public static int Particles { get; internal set; }

        public static SpriteFont Font { get; set; }

        public static void Draw(ScrapVector offset)
        {
            if (Font == null)
                return;

            Vector2 TextOffset = Font.MeasureString("Render Diagnostics");
            Renderer.RenderText(Font, "Render Diagnostics", offset, Color.White);
            Renderer.RenderText(Font, $"Renderer calls: {Calls}", offset + new ScrapVector(0, TextOffset.Y), Color.White);
            Renderer.RenderText(Font, $"Total sprites: {SpriteSystem.Sprites}", offset + new ScrapVector(0, TextOffset.Y * 2), Color.White);
            Renderer.RenderText(Font, $"Sprite system time taken: {SpriteSystem.TimeTaken} ms", offset + new ScrapVector(0, TextOffset.Y * 3), Color.White);
        }
    }
}
