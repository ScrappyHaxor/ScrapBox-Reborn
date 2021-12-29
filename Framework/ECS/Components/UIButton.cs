//using System;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Graphics;

//using ScrapBox.Framework.Scene;
//using ScrapBox.Framework.Managers;
//using ScrapBox.Framework.Services;
//using ScrapBox.Framework.Math;

//namespace ScrapBox.Framework.ECS.Components
//{
//	public class UIButton : IComponent
//	{
//		public Entity Owner { get; set; }
//		public bool IsAwake { get; set; }
//		public Transform Transform { get; set; }
			
//		public Color BackColor { get; set; }
//		public Color HoverColor { get; set; }
//		public Color BorderColor { get; set; }

//		protected bool hovered;
//		protected ButtonState lastState;

//		public EventHandler Pressed;

//		public UIButton()
//		{
			
//		}

//		public virtual void Awake()
//		{
//			Transform = Owner.GetComponent<Transform>();
//			if (Transform == null)
//			{
//				LogService.Log("Sprite2D", "Awake", "Missing dependency. Requires transform component to work.", Severity.ERROR);
//				return;
//			}

//			if (!Transform.IsAwake)
//			{
//				LogService.Log("Sprite2D", "Awake", "Transform component is not awake... Aborting...", Severity.ERROR);
//				return;
//			}

//			IsAwake = true;
//		}

//		public virtual void Sleep()
//        {
//			IsAwake = false;
//        }

//		public virtual void Update(double dt)
//		{
//			MouseState mouseState = Mouse.GetState();
//			if (mouseState.X >= Transform.Position.X - Transform.Dimensions.X / 2 && mouseState.Y >= Transform.Position.Y - Transform.Dimensions.Y / 2 &&
//				mouseState.X <= Transform.Position.X - Transform.Dimensions.X / 2 + Transform.Dimensions.X && 
//				mouseState.Y <= Transform.Position.Y - Transform.Dimensions.Y / 2 + Transform.Dimensions.Y)
//			{
//				hovered = true;
//			}
//			else
//			{
//				hovered = false;
//			}

//			if (hovered && mouseState.LeftButton == ButtonState.Pressed && lastState != ButtonState.Pressed)
//			{
//				Pressed?.Invoke(this, null);
//			}

//			lastState = mouseState.LeftButton;
//		}

//		public virtual void Draw(Camera camera)
//		{
//			if (!hovered)
//            {
//				Renderer.RenderBox(Transform.Position, Transform.Dimensions, (float)Transform.Rotation, BackColor, camera);
//			}
//            else
//            {
//				Renderer.RenderBox(Transform.Position, Transform.Dimensions, (float)Transform.Rotation, HoverColor, camera);
//			}

//			Renderer.RenderBox(new ScrapVector(Transform.Position.X, Transform.Position.Y - Transform.Dimensions.Y / 2),
//				new ScrapVector(Transform.Dimensions.X, 4), 0, BorderColor, camera);

//			Renderer.RenderBox(new ScrapVector(Transform.Position.X, Transform.Position.Y + Transform.Dimensions.Y / 2),
//				new ScrapVector(Transform.Dimensions.X, 4), 0, BorderColor, camera);

//			Renderer.RenderBox(new ScrapVector(Transform.Position.X - Transform.Dimensions.X / 2, Transform.Position.Y),
//				new ScrapVector(4, Transform.Dimensions.Y), 0, BorderColor, camera);

//			Renderer.RenderBox(new ScrapVector(Transform.Position.X + Transform.Dimensions.X / 2, Transform.Position.Y),
//				new ScrapVector(4, Transform.Dimensions.Y), 0, BorderColor, camera);
//		}
//	}
//}
