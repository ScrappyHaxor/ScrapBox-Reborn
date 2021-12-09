using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Scene;


namespace ScrapBox.Framework.ECS.Components
{
	public class RigidBody2D : IComponent
	{
		public enum RigidState
		{
			REST_STATIC,
			REST_DYNAMIC,
			FALLING
		}

		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }
		public ScrapVector Force { get; internal set; }
		public double Torque { get; internal set; }

		public bool Kinematic { get; set; }

		public RigidState State { get; internal set; }

		public double Mass { get; set; }
		public double InverseMass { get { if (IsStatic) { return 0; } return 1 / Mass; } }

		public double I { get; set; }
		public double InverseI { get { if (IsStatic) { return 0; } return 1 / I; } }

		public ScrapVector LinearVelocity { get; set; }
		public double AngularVelocity { get; set; }
		public double Restitution { get; set; }
		public bool IsStatic { get; set; }
		public double Friction { get; set; }
		public double Drag { get; set; }
		
		public RigidBody2D()
		{
			State = RigidState.FALLING;
		}
		
		public virtual void AddForce(ScrapVector force)
		{

			Force += force;
		}

		internal virtual void ApplyForces(double dt, double iterations)
		{
			if (!IsAwake)
				return;

			ScrapVector acceleration = ScrapVector.Zero;
			acceleration += Force / Mass;

			if (State != RigidState.REST_STATIC && State != RigidState.REST_DYNAMIC)
			{
				acceleration += Physics2D.Gravity / Mass;
			}
			
			LinearVelocity += acceleration * dt / iterations;
			
			if (State != RigidState.REST_STATIC && State != RigidState.REST_DYNAMIC)
			{
				if (Physics2D.Gravity == ScrapVector.Zero)
				{
					LinearVelocity *= new ScrapVector(Drag, Drag);
				}
				else
				{
					LinearVelocity *= new ScrapVector(Drag, 1);
				}
			}
			else
			{
				if (Physics2D.Gravity == ScrapVector.Zero)
				{
					LinearVelocity *= new ScrapVector(Friction, Friction);
				}
				else
				{
					LinearVelocity *= new ScrapVector(Friction, 1);
				}
			}
						
			Transform.Position += LinearVelocity * dt / iterations;

			Force = ScrapVector.Zero;
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

			I = Mass * (Transform.Dimensions.X * Transform.Dimensions.X + Transform.Dimensions.Y * Transform.Dimensions.Y) / 12.0;

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
		}

		public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake)
				return;
		}
	}
}
