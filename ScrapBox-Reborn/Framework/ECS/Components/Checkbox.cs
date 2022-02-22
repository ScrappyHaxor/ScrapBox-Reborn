using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.Shapes;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Input;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
    public class Checkbox : Interface
    {
        public override string Name => "Checkbox";

        public Color BoxColor;
        public Color CheckColor;
        public Color TextColor;

        public double CheckThickness;

        public SpriteFont Font;

        public string BoxText;
        public bool Checked { get; set; }
        private ScrapShape box;

        public override void Awake()
        {
            if (IsAwake)
                return;

            if (Font == null)
            {
                LogService.Log(Name, "Awake", "No font provided.", Severity.ERROR);
                return;
            }

            base.Awake();

            box = ScrapRect.CreateFromCenter(Transform.Position, Transform.Dimensions);

            if (BoxColor == default)
            {
                BoxColor = Color.White;
            }

            if (CheckColor == default)
            {
                CheckColor = BoxColor;
            }

            if (TextColor == default)
            {
                TextColor = BoxColor;
            }

            if (BoxText == null)
            {
                BoxText = "Lorem Ipsum";
            }

            if (CheckThickness == 0)
            {
                CheckThickness = 1;
            }
        }

        internal override void Tick()
        {
            box.Center = Transform.Position;
            box.Dimensions = Transform.Dimensions;

            if (Collision.IntersectPointPolygon(InputManager.GetMouseWorldPosition(WorldManager.CurrentScene.MainCamera), box.GetVerticies()))
            {
                if (InputManager.IsButtonDown(Input.Button.LEFT_MOUSE_BUTTON))
                {
                    Checked = !Checked;
                }
            }

            base.Tick();
        }

        internal override void Render(Camera mainCamera)
        {
            ScrapVector textDims = Renderer.MeasureText(Font, BoxText);
            Renderer.RenderCenteredText(Font, BoxText, new ScrapVector(Transform.Position.X - textDims.X / 2 - Transform.Dimensions.X, Transform.Position.Y), TextColor, mainCamera);

            if (Checked)
            {
                Renderer.RenderLine(Transform.Position - Transform.Dimensions / 2, Transform.Position + Transform.Dimensions / 2,
                    CheckColor, mainCamera, thickness: CheckThickness);
                Renderer.RenderLine(
                    new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2, Transform.Position.Y - Transform.Dimensions.Y / 2),
                    new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2, Transform.Position.Y + Transform.Dimensions.Y / 2),
                    CheckColor, mainCamera, thickness: CheckThickness);
            }

            Renderer.RenderPolygonOutline(box.GetVerticies(), BoxColor, mainCamera);

            base.Render(mainCamera);
        }
    }
}
