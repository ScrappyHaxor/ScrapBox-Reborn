using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public int MaxValue = 0;
        public int MinValue = 0;

        public ProgressBar()
        {

        }

        public void SetValue()
        {

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
                BackgroundColor = Color.DarkGray;
            }

            if(MaxValue == 0)
            {
                MaxValue = 100;
            }

            Foreground = new Rectangle(Transform.Position, Transform.Dimensions);
            Background = new Rectangle(Transform.Position, Transform.Dimensions);
        }
    }
}
