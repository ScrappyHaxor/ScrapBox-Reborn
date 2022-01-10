using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Services;
using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.ECS.Components
{
    public class Label : Interface
    {
        public override string Name => "Label";

        public SpriteFont Font;
        public string Text;
        public Color TextColor;

        public override void Awake()
        {
            if (IsAwake)
                return;

            if (Font == null)
            {
                LogService.Log(Name, "Awake", "No font provided.", Severity.ERROR);
                return;
            }

            if (Text == null)
            {
                Text = "Lorem Ipsum";
            }

            if (TextColor == default)
            {
                TextColor = Color.White;
            }

            base.Awake();
        }

        internal override void Render(Camera mainCamera)
        {
            Renderer.RenderCenteredText(Font, Text, Transform.Position, TextColor, mainCamera);
            base.Render(mainCamera);
        }
    }
}
