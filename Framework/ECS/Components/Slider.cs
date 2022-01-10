using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Level;
using ScrapBox.Framework.Shapes;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Input;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Generic;

namespace ScrapBox.Framework.ECS.Components
{
    public class Slider : Interface
    {
        public const int DEFAULT_HANDLE_POINTS = 22;
        public const int SLIDER_BOUND_OFFSET = 5;

        public override string Name => "Slider";

        public Color BarColor { get; set; }
        public Color BarBorderColor { get; set; }
        public Color HandleColor { get; set; }
        public Color HandleHoverColor { get; set; }
        public double HandleRadius { get; set; }
        public int HandlePoints { get; set; }
        public int Value { get; private set; }
        public SpriteFont Font { get; set; }
        public string Label { get; set; }

        public int UpperBound;
        public int LowerBound;

        private ScrapCircle handle;
        private double handleX;

        private bool hovered;
        private bool dragging;

        private double MiddleRatio(int minBound, int maxBound)
        {
            double middle = (maxBound + minBound) / 2;

            return 0;
        }

        public void SetValue(int value)
        {
            value = (int)ScrapMath.Clamp(value, LowerBound, UpperBound);

            double totalLength = ScrapMath.Length(new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2, 0) -
                new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2, 0));

            double val;
            if (LowerBound < 0)
            {
                val = UpperBound + ScrapMath.Abs(LowerBound);
                double ratio = (value - LowerBound) / val;
                handleX = ratio * totalLength + Transform.Position.X - totalLength * 0.5;
            }
            else
            {
                val = UpperBound - LowerBound;
                double ratio = (value - LowerBound) / val;
                handleX = ratio * totalLength + Transform.Position.X - totalLength * 0.5;
            }
        }

        public override void Awake()
        {
            if (Font == null)
            {
                LogService.Log(Name, "Awake", "Font is null.", Severity.ERROR);
                return;
            }

            base.Awake();

            if (BarColor == default)
            {
                BarColor = Color.Black;
            }

            if (BarBorderColor == default)
            {
                BarBorderColor = BarColor;
            }

            if (HandleColor == default)
            {
                HandleColor = Color.White;
            }

            if (HandleHoverColor == default)
            {
                HandleHoverColor = HandleColor;
            }

            if (HandlePoints == 0)
            {
                HandlePoints = DEFAULT_HANDLE_POINTS;
            }

            if (UpperBound == 0)
            {
                UpperBound = 1;
            }

            if (LowerBound > UpperBound)
            {
                Standard.Swap(ref LowerBound, ref UpperBound);
            }

            if (Label == null)
            {
                Label = "Lorem Ipsum";
            }

            handleX = Transform.Position.X;

            handle = new ScrapCircle(new ScrapVector(handleX, Transform.Position.Y), HandleRadius, HandlePoints);
        }

        internal override void Tick()
        {
            ScrapVector mouseWorld = InputManager.GetMouseWorldPosition(WorldManager.CurrentScene.MainCamera);
            if (Collision.IntersectPointPolygon(mouseWorld, handle.GetVerticies()))
            {
                hovered = true;
                if (InputManager.IsButtonHeld(Input.Button.LEFT_MOUSE_BUTTON))
                {
                    dragging = true;
                }
                else
                {
                    dragging = false;
                }
            }
            else
            {
                hovered = false;
            }

            if (dragging && !InputManager.IsButtonHeld(Input.Button.LEFT_MOUSE_BUTTON))
            {
                dragging = false;
            }

            if (dragging)
            {
                handleX = mouseWorld.X;
            }

            handleX = ScrapMath.Clamp(handleX, Transform.Position.X - Transform.Dimensions.X / 2, Transform.Position.X + Transform.Dimensions.X / 2);

            handle.Center = new ScrapVector(handleX, Transform.Position.Y);

            double totalLength = ScrapMath.Length(new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2, 0) - 
                new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2, 0));
            double handleLength = ScrapMath.Length(new ScrapVector(handleX, 0) - new ScrapVector(Transform.Position.X - 
                Transform.Dimensions.X / 2, 0));

            double ratio = handleLength / totalLength;

            double val = UpperBound - LowerBound;
            Value = (int)ScrapMath.Round(ratio * val + LowerBound);

            base.Tick();
        }

        internal override void Render(Camera mainCamera)
        {
            Renderer.RenderBox(Transform.Position, Transform.Dimensions, 0, BarColor, mainCamera);
            Renderer.RenderOutlineBox(Transform.Position, Transform.Dimensions, 0, BarBorderColor, mainCamera);
            
            if (hovered || dragging)
            {
                Renderer.RenderPolygonOutline(handle.GetVerticies(), HandleHoverColor, mainCamera);
            }
            else
            {
                Renderer.RenderPolygonOutline(handle.GetVerticies(), HandleColor, mainCamera);
            }

            ScrapVector upperTextDims = Renderer.MeasureText(Font, UpperBound.ToString());
            ScrapVector lowerTextDims = Renderer.MeasureText(Font, LowerBound.ToString());
            Renderer.RenderCenteredText(Font, UpperBound.ToString(),
                new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2 + upperTextDims.X / 2 + SLIDER_BOUND_OFFSET, Transform.Position.Y),
                HandleColor, mainCamera);
            Renderer.RenderCenteredText(Font, LowerBound.ToString(),
                new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2 - lowerTextDims.X / 2 - SLIDER_BOUND_OFFSET, Transform.Position.Y),
                HandleColor, mainCamera);

            ScrapVector valueTextDims = Renderer.MeasureText(Font, Value.ToString());
            Renderer.RenderCenteredText(Font, Value.ToString(),
                new ScrapVector(handleX, Transform.Position.Y + Transform.Dimensions.Y / 2 + valueTextDims.Y / 2 + SLIDER_BOUND_OFFSET),
                HandleColor, mainCamera);

            ScrapVector labelDims = Renderer.MeasureText(Font, Label);
            Renderer.RenderCenteredText(Font, Label,
                new ScrapVector(Transform.Position.X, Transform.Position.Y - Transform.Dimensions.Y / 2 - labelDims.Y / 2 - SLIDER_BOUND_OFFSET),
                HandleColor, mainCamera);

            base.Render(mainCamera);
        }
    }
}
