using System.Diagnostics;
using System.Collections.Generic;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.ECS.Components;
using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.ECS.Systems
{
    public class SpriteSystem : ComponentSystem
    {
        public static int Sprites;
        public static double TimeTaken;

        private readonly Stopwatch watch;

        private readonly List<Sprite2D> sprites;

        public SpriteSystem()
        {
            watch = new Stopwatch();

            sprites = new List<Sprite2D>();
        }

        public void RegisterSprite(Sprite2D sprite)
        {
            sprites.Add(sprite);
            Sprites++;
        }

        public void PurgeSprite(Sprite2D sprite)
        {
            sprites.Remove(sprite);
            Sprites--;
        }

        public override void Reset()
        {
            sprites.Clear();
        }

        public override void Update(double dt)
        {
            
        }

        public override void Draw(Camera mainCamera)
        {
            watch.Restart();
            foreach (Sprite2D sprite in sprites)
            {
                if (!sprite.IsAwake)
                    continue;

                Renderer.RenderSprite(sprite, mainCamera);
            }

            TimeTaken = watch.ElapsedMilliseconds / 1000;
        }
    }
}
