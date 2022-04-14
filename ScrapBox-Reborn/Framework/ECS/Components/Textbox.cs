using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Shapes;
using ScrapBox.Framework.Level;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Input;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
    public class Textbox : Interface
    {
        public override string Name => "Textbox";

        public const double DEFAULT_HORIZONTAL_PADDING = 10;
        public const double ERASE_COOLDOWN = 80;

        public string Placeholder;
        public ScrapShape Shape;
        public SpriteFont Font;
        public double HorizontalPadding;
        public bool ReadOnly;
        public bool Multiline;
        public double OutlineThickness;

        public Color BorderColor;
        public Color FocusColor;
        public Color PlaceholderColor;

        private bool focused;
        public string Input;

        private double lastErase;

        public Textbox()
        {
            OutlineThickness = 1;
        }

        private void ProcessInput()
        {
            List<Keys> keyList = InputManager.GetKeysDown();
            foreach (Keys k in keyList)
            {
                if (k == Keys.Space)
                {
                    Input = $"{Input} ";
                }

                if ((int)k >= 65 && (int)k <= 90 || (int)k >= 97 && (int)k <= 122)
                {
                    if (Renderer.MeasureText(Font, Input).X >= Transform.Dimensions.X - HorizontalPadding * 3)
                        return;

                    if (InputManager.IsKeyHeld(Keys.LeftShift))
                    {
                        Input = $"{Input}{(char)k}";
                    }
                    else
                    {
                        Input = $"{Input}{char.ToLower((char)k)}";
                    }
                }
            }
        }

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

            if (Placeholder == null)
            {
                Placeholder = "Lorem Ipsum";
            }

            if (Shape == null)
            {
                Shape = ScrapRect.CreateFromCenter(Transform.Position, Transform.Dimensions);
            }

            if (BorderColor == default)
            {
                BorderColor = Color.White;
            }

            if (FocusColor == default)
            {
                FocusColor = BorderColor;
            }

            if (HorizontalPadding == 0)
            {
                HorizontalPadding = DEFAULT_HORIZONTAL_PADDING;
            }

            if (PlaceholderColor == default)
            {
                PlaceholderColor = Color.White;
            }

            lastErase = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Input = string.Empty;
        }

        internal override void Tick()
        {
            Shape.Center = Transform.Position;
            Shape.Dimensions = Transform.Dimensions;

            if (!ReadOnly)
            {
                if (Collision.IntersectPointPolygon(InputManager.GetMouseWorldPosition(SceneManager.CurrentScene.MainCamera), Shape.Verts))
                {
                    focused = true;
                }
                else
                {
                    focused = false;
                }

                if (focused)
                {
                    if (InputManager.IsKeyHeld(Keys.Back) && DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastErase > ERASE_COOLDOWN)
                    {
                        if (Input.Length != 0)
                        {
                            Input = Input[0..^1];
                            lastErase = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        }
                    }

                    ProcessInput();
                }
            }

            base.Tick();
        }

        internal override void Render(Camera mainCamera)
        {
            if (focused)
            {
                Renderer.RenderPolygonOutline(Shape.Verts, FocusColor, mainCamera, null, OutlineThickness);
            }
            else
            {
                Renderer.RenderPolygonOutline(Shape.Verts, BorderColor, mainCamera, null, OutlineThickness);
            }

            ScrapVector dims;
            if (Input == string.Empty)
            {
                dims = Renderer.MeasureText(Font, Placeholder);
                Renderer.RenderText(Font, Placeholder, new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2 + HorizontalPadding,
                    Transform.Position.Y - dims.Y / 2), PlaceholderColor, mainCamera);
            }
            else
            {
                dims = Renderer.MeasureText(Font, Input);
                Renderer.RenderText(Font, Input, new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2 + HorizontalPadding, 
                    Transform.Position.Y - dims.Y / 2), Color.White, mainCamera);
            }    
            
            
            base.Render(mainCamera);
        }
    }
}
