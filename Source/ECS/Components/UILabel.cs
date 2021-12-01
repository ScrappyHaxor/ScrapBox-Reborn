using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ScrapBox.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using ScrapBox.Managers;
using ScrapBox.SMath;

namespace ScrapBox.ECS.Components
{
    public class UILabel : IComponent
    {
        public Entity Owner { get; set; }
        public bool IsAwake { get; set; }
        public Transform Transform { get; set; }

        public string Label { get; set; }
        public SpriteFont Font { get; set; }
        public Color TextColor { get; set; }

        public UILabel()
        {

        }

        public void Awake()
        {
            Transform = Owner.GetComponent<Transform>();
            if (Transform == null)
            {
                LogManager.Log(new LogMessage("Sprite2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
                return;
            }

            if (!Transform.IsAwake)
            {
                LogManager.Log(new LogMessage("Sprite2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
                return;
            }

            IsAwake = true;
        }

        public void Update(double dt)
        {
            if (!IsAwake)
                return;


        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!IsAwake)
                return;

            if (Font == null)
                return;

            Renderer2D.RenderText(Font, Label, Transform.Position - new ScrapVector(Font.MeasureString(Label)/2), TextColor);
        }
    }
}
