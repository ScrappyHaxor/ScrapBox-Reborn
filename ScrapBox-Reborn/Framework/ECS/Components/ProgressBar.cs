using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;

using Rectangle = ScrapBox.Framework.Shapes.Rectangle;

namespace ScrapBox.Framework.ECS.Components
{
    public class ProgressBar : Interface
    {
        public override string Name => "Progress Bar";

        public Color BorderColor;
        public Color ForegroundColor;
        public Color BackgroundColor;

        public Rectangle Foreground;
        public Rectangle Background;

        public int MaxValue { get; private set; }
        public int MinValue { get; private set; }
        public double CurrentValue { get; private set; }

        public ProgressBar(int maxValue, int minValue = 0)
        {

        }

        public void SetValue(double value)
        {
            CurrentValue += value;
        }

        public void Reset()
        {
            //it is set to 2 due to an error with the transform
            CurrentValue = 2;
            Foreground = new Rectangle(Transform.Position, new ScrapVector(CurrentValue, Transform.Dimensions.Y));
        }

        public override void Awake()
        {
            base.Awake();

            if(ForegroundColor == default)
            {
                ForegroundColor = Color.Green;
            }

            if(BackgroundColor == default)
            {
                BackgroundColor = Color.Blue;
            }

            if(MaxValue == 0)
            {
                MaxValue = 300;
            }

            Foreground = new Rectangle(Transform.Position, new ScrapVector(CurrentValue, Transform.Dimensions.Y));
            Background = new Rectangle(Transform.Position, new ScrapVector(MaxValue, Transform.Dimensions.Y));
        }

        internal override void Tick()
        {
            Foreground.Dimensions = new ScrapVector(CurrentValue, Transform.Dimensions.Y);

            base.Tick();
        }

        internal override void Render(Camera mainCamera)
        {
            //  temporary workaround due to errors caused by transform or renderer.
            //  the rectangle is not drawn if the X-value of its transform is set to 0.
            //  the program gets stuck in a loop if the X-value is set to 1.
            if(CurrentValue < 3)
            {
                ForegroundColor = Color.Transparent;
            }
            else
            {
                ForegroundColor = Color.Green;
            }

            TriangulationService.Triangulate(Background.Verticies, TriangulationMethod.EAR_CLIPPING, out int[] backIndicies);
            Renderer.RenderPolygon(Background.Verticies, backIndicies, BackgroundColor, mainCamera);

            TriangulationService.Triangulate(Foreground.Verticies, TriangulationMethod.EAR_CLIPPING, out int[] foreIndicies);
            Renderer.RenderPolygon(Foreground.Verticies, foreIndicies, ForegroundColor, mainCamera);

            base.Render(mainCamera);
        }
    }
}
