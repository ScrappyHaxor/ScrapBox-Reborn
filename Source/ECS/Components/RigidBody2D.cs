using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;

using System;

namespace ScrapBox.ECS.Components
{
	public class RigidBody2D : IComponent
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }

		public Vector2 Velocity { get; set; }
		public Vector2 Force { get; set; }
		public float Mass { get; set; }

		public Vector2 LinearVelocity { get; set; }
		public float Density { get; set; }
		public float Restitution { get; set; }
		public bool IsStatic { get; set; }
		
		public RigidBody2D()
		{

		}
		
		public virtual void AddForce(Vector2 force)
		{
			Force += force;
		}

		public virtual void Awake()
		{
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("RigidBody2D", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("RigidBody2D", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}

			if (Mass == 0)
				LogManager.Log(new LogMessage("RigidBody2D", "Mass is 0. This will cause unexpected behaviour", LogMessage.Severity.WARNING));

			IsAwake = true;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (!IsAwake)
				return;

			Force += Physics2D.Gravity * Mass;
			Velocity += Force / Mass * (float)gameTime.TotalGameTime.TotalSeconds;
			Transform.Position += Velocity * (float)gameTime.TotalGameTime.TotalSeconds;

			Force = Vector2.Zero;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsAwake)
				return;
		}
	}
}
