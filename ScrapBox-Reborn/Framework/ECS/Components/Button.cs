using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Shapes;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Input;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Managers;
using Rectangle = ScrapBox.Framework.Shapes.Rectangle;

namespace ScrapBox.Framework.ECS.Components
{
    public class Button : Interface
    {
        public override string Name => "Button";

        public bool Fill;
        public Color FillColor;
        public Color BorderColor;
        public Color HoverColor;
        public Polygon Shape;
        public double OutlineThickness;

        public EventHandler Pressed;

        public bool Hovered;

        private int[] indicies;

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
                Shape = new Rectangle(Transform.Position, Transform.Dimensions);
            }

            if (FillColor == default)
            {
                FillColor = Color.Transparent;
            }

            if (BorderColor == default)
            {
                BorderColor = Color.Transparent;
            }

            if (HoverColor == default)
            {
                HoverColor = Color.Transparent;
            }
        }

        internal override void Tick()
        {
            Shape.Position = Transform.Position;
            Shape.Dimensions = Transform.Dimensions;
            TriangulationService.Triangulate(Shape.Verticies, TriangulationMethod.EAR_CLIPPING, out indicies);

            if (Collision.IntersectPointPolygon(InputManager.GetMouseWorldPosition(SceneManager.CurrentScene.MainCamera), Shape.Verticies))
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
                Renderer.RenderPolygon(Shape.Verticies, indicies, HoverColor, null, mainCamera);

                Renderer.RenderPolygonOutline(Shape.Verticies, HoverColor, mainCamera, null, OutlineThickness);
            }
            else
            {
                Renderer.RenderPolygon(Shape.Verticies, indicies, FillColor, null, mainCamera);

                Renderer.RenderPolygonOutline(Shape.Verticies, BorderColor, mainCamera, null, OutlineThickness);
            }

            base.Render(mainCamera);
        }
    }
}
