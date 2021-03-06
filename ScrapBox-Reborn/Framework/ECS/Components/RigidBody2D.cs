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


		public Transform2D Transform;

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

		public bool Grounded()
        {
			return (State == RigidState.REST_STATIC || State == RigidState.REST_DYNAMIC) ? true : false;
        }

		public bool Bounce()
		{
			if (Layer == null)
				return false;

			CollisionSystem collisionSystem = Layer.GetSystem<CollisionSystem>();
			if (collisionSystem == null)
				return false;

			double topY = Transform.Position.X - Transform.Dimensions.X / 2;
			double bottomY = Transform.Position.X + Transform.Dimensions.X / 2;
			double middleY = Transform.Position.Y / 2;
			double leftX = Transform.Position.X - Transform.Dimensions.X / 2;
			double rightX = Transform.Position.X + Transform.Dimensions.X / 2;

			RayResult middleLeft = collisionSystem.Raycast(new PointRay(new ScrapVector(leftX, middleY)));
			RayResult middleRight = collisionSystem.Raycast(new PointRay(new ScrapVector(rightX, middleY)));
			RayResult topLeft = collisionSystem.Raycast(new PointRay(new ScrapVector(leftX, topY)));
			RayResult topRight = collisionSystem.Raycast(new PointRay(new ScrapVector(rightX, topY)));
			RayResult bottomLeft = collisionSystem.Raycast(new PointRay(new ScrapVector(leftX, bottomY)));
			RayResult bottomRight = collisionSystem.Raycast(new PointRay(new ScrapVector(rightX, bottomY)));

			return middleLeft.hit || middleRight.hit || topLeft.hit || topRight.hit || bottomLeft.hit || bottomRight.hit;
		}

		internal void ApplyForces(double dt, double iterations)
		{
			if (!IsAwake)
				return;

			ScrapVector acceleration = PhysicsSystem.Gravity;
			acceleration += Force / Mass;
			Force = ScrapVector.Zero;

			if (State != RigidState.REST_STATIC && State != RigidState.REST_DYNAMIC)
			{
				acceleration += LinearVelocity * -Drag;
			}
			else
			{
				acceleration += LinearVelocity * -Friction;
			}

			LinearVelocity += acceleration * dt;
			Transform.Position += LinearVelocity;
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

			if (Layer == null)
				Layer = Owner.Layer;

			Layer.GetSystem<PhysicsSystem>().RegisterBody(this);
			IsAwake = true;
		}

		public override void Sleep()
        {
			if (!IsAwake)
				return;

			Layer.GetSystem<PhysicsSystem>().PurgeBody(this);
			IsAwake = false;
        }
    }
}
