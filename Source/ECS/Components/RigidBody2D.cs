using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;
using ScrapBox.SMath;

using System;


namespace ScrapBox.ECS.Components
{
	public class RigidBody2D : IComponent
	{
		public enum RigidState
		{
			REST,
			APPLYING_FORCE
		}

		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }

		public RigidState State { get; set; }
		public ScrapVector Velocity { get; set; }
		public float Mass { get; set; }

		public List<ScrapVector> Forces { get; internal set; }

		public float InverseMass { get { if (IsStatic) { return 0; } return 1 / Mass; } }

		public ScrapVector LinearVelocity { get; set; }
		public ScrapVector AngularVelocity { get; set; }
		public float Density { get; set; }
		public float Restitution { get; set; }
		public bool IsStatic { get; set; }
		public float Friction { get; set; }
		
		public RigidBody2D()
		{
			Forces = new List<ScrapVector>();
			State = RigidState.APPLYING_FORCE;
		}
		
		public virtual void AddForce(ScrapVector force)
		{
			Forces.Add(force);
			State = RigidState.APPLYING_FORCE;
		}

		internal virtual void ApplyForces(GameTime gameTime)
		{
			if (!IsAwake)
				return;

			ScrapVector acceleration = ScrapVector.Zero;
			foreach (ScrapVector force in Forces)
			{
				acceleration += force / Mass;
			}

			acceleration += Physics2D.Gravity / Mass;

			Velocity += acceleration * gameTime.ElapsedGameTime.TotalSeconds;

			Forces.Clear();
		}

		internal virtual void ApplyVelocities(GameTime gameTime)
		{
			if (!IsAwake)
				return;

			Transform.Position += Velocity * gameTime.ElapsedGameTime.TotalSeconds;
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

			if (Mass == 0 && !IsStatic)
				LogManager.Log(new LogMessage("RigidBody2D", "Mass is 0 on dynamic body. This will cause unexpected behaviour", LogMessage.Severity.WARNING));

			IsAwake = true;
		}

		public virtual void Update(GameTime gameTime)
		{
			if (!IsAwake)
				return;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsAwake)
				return;
		}
	}
}
