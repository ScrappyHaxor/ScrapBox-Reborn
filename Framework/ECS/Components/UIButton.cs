using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Scene;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
	public class UIButton : IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }
		public Transform Transform { get; set; }
			
		public Color BackColor { get; set; }
		public Color HoverColor { get; set; }
		public Color BorderColor { get; set; }

		protected bool hovered;
		protected ButtonState lastState;

		public EventHandler Pressed;

		public UIButton()
		{
			
		}

		public virtual void Awake()
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

		public virtual void Sleep()
        {
			IsAwake = false;
        }

		public virtual void Update(double dt)
		{
			if (!IsAwake) 
				return;

			MouseState mouseState = Mouse.GetState();
			if (mouseState.X >= Transform.Position.X - Transform.Dimensions.X / 2 && mouseState.Y >= Transform.Position.Y - Transform.Dimensions.Y / 2 &&
				mouseState.X <= Transform.Position.X - Transform.Dimensions.X / 2 + Transform.Dimensions.X && 
				mouseState.Y <= Transform.Position.Y - Transform.Dimensions.Y / 2 + Transform.Dimensions.Y)
			{
				hovered = true;
			}
			else
			{
				hovered = false;
			}

			if (hovered && mouseState.LeftButton == ButtonState.Pressed && lastState != ButtonState.Pressed)
			{
				Pressed?.Invoke(this, null);
			}

			lastState = mouseState.LeftButton;
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake)
				return;

			if (!hovered)
            {
				Renderer2D.RenderPrimitiveBox(Transform.Position, Transform.Dimensions, Transform.Rotation, BackColor, camera);
			}
            else
            {
				Renderer2D.RenderPrimitiveBox(Transform.Position, Transform.Dimensions, Transform.Rotation, HoverColor, camera);
			}

			Renderer2D.RenderPrimitiveBox(new ScrapVector(Transform.Position.X, Transform.Position.Y - Transform.Dimensions.Y / 2),
				new ScrapVector(Transform.Dimensions.X, 4), 0, BorderColor, camera);

			Renderer2D.RenderPrimitiveBox(new ScrapVector(Transform.Position.X, Transform.Position.Y + Transform.Dimensions.Y / 2),
				new ScrapVector(Transform.Dimensions.X, 4), 0, BorderColor, camera);

			Renderer2D.RenderPrimitiveBox(new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2, Transform.Position.Y),
				new ScrapVector(4, Transform.Dimensions.Y), 0, BorderColor, camera);

			Renderer2D.RenderPrimitiveBox(new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2, Transform.Position.Y),
				new ScrapVector(4, Transform.Dimensions.Y), 0, BorderColor, camera);
		}
	}
}
