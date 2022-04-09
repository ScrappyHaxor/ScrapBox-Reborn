using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Shapes;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Input;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;

namespace ScrapBox.Framework.ECS.Components
{
    public class Button : Interface
    {
        public override string Name => "Button";

        public bool Fill;
        public Color FillColor;
        public Color BorderColor;
        public Color HoverColor;
        public ScrapShape Shape;
        public double OutlineThickness;

        public EventHandler Pressed;

        public bool Hovered;

        public Button()
        {
            OutlineThickness = 1;
        }

        public override void Awake()
        {
            if (IsAwake)
                return;

            base.Awake();

            if (Shape == null)
            {
                Shape = ScrapRect.CreateFromCenter(Transform.Position, Transform.Dimensions);
            }

            if (FillColor == default)
            {
                FillColor = Color.White;
            }

            if (BorderColor == default)
            {
                BorderColor = FillColor;
            }

            if (HoverColor == default)
            {
                HoverColor = FillColor;
            }
        }

        internal override void Tick()
        {
            Shape.Center = Transform.Position;
            Shape.Dimensions = Transform.Dimensions;
            if (Collision.IntersectPointPolygon(InputManager.GetMouseWorldPosition(WorldManager.CurrentScene.MainCamera), Shape.Verts))
            {
                Hovered = true;
                if (InputManager.IsButtonDown(Input.Button.LEFT_MOUSE_BUTTON))
                {
                    Pressed?.Invoke(this, null);
                }
            }
            else
            {
                Hovered = false;
            }

            base.Tick();
        }

        internal override void Render(Camera mainCamera)
        {
            if (Hovered)
            {
                Renderer.RenderPolygonOutline(Shape.GetVerticies(), HoverColor, mainCamera, null, OutlineThickness);
            }
            else
            {
                Renderer.RenderPolygonOutline(Shape.GetVerticies(), BorderColor, mainCamera, null, OutlineThickness);
            }

            base.Render(mainCamera);
        }
    }
}
