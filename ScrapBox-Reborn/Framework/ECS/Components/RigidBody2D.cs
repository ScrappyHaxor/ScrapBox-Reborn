using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.ECS.Systems;

namespace ScrapBox.Framework.ECS.Components
{
	public class RigidBody2D : Component
	{
		public enum RigidState
		{
			REST_STATIC,
			REST_DYNAMIC,
			FALLING
		}

		public override string Name => "Rigidbody2D";


		public Transform Transform;

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
		
		public void AddForce(ScrapVector force)
		{
			Force += force;
		}

		internal void ApplyForces(double dt, double iterations)
		{
			if (!IsAwake)
				return;

			ScrapVector acceleration = ScrapVector.Zero;
			acceleration += Force / Mass;

			if (State != RigidState.REST_STATIC && State != RigidState.REST_DYNAMIC)
			{
				acceleration += PhysicsSystem.Gravity / Mass;
			}
			
			LinearVelocity += acceleration * dt / iterations;
			
			if (State != RigidState.REST_STATIC && State != RigidState.REST_DYNAMIC)
			{
				if (PhysicsSystem.Gravity == ScrapVector.Zero)
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
				if (PhysicsSystem.Gravity == ScrapVector.Zero)
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

		public override void Awake()
		{
			if (IsAwake)
				return;

			bool success = Dependency(out Transform);
			if (!success)
				return;

			if (Mass == 0 && !IsStatic)
            {
				LogService.Log(Name, "Awake", "Mass is 0 on non-static object.", Severity.ERROR);
				return;
            }

			I = Mass * (Transform.Dimensions.X * Transform.Dimensions.X + Transform.Dimensions.Y * Transform.Dimensions.Y) / 12.0;

			WorldManager.GetSystem<PhysicsSystem>().RegisterBody(this);
			IsAwake = true;
		}

		public override void Sleep()
        {
			if (!IsAwake)
				return;

			WorldManager.GetSystem<PhysicsSystem>().PurgeBody(this);
			IsAwake = false;
        }
    }
}
