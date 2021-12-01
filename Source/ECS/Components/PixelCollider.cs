using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Managers;
using ScrapBox.SMath;
using ScrapBox.Scene;
using ScrapBox.Args;

using System;

namespace ScrapBox.ECS.Components
{
	public class PixelCollider : ICollider
	{
		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }

		public Transform Transform { get; set; }
		public RigidBody2D RigidBody { get; set; }
		public ScrapVector Dimensions { get; set; }
		public Sprite2D Sprite { get; set; }
		public ICollider.ColliderType TypeOfCollider { get; set; }
		public ICollider.TriggerType Trigger { get; set; }
		public EventHandler<TriggerArgs> Triggered { get; set; }

		public bool HasRigidbody { get { return Owner.HasComponent<RigidBody2D>(); } }

		public double Top { get { return Transform.Position.Y - Dimensions.Y / 2; } }
		public double Bottom { get { return Transform.Position.Y + Dimensions.Y / 2; } }
		public double Left { get { return Transform.Position.X - Dimensions.X / 2; } }
		public double Right { get { return Transform.Position.X + Dimensions.X / 2; } }

		public virtual ScrapVector[] GetVerticies()
		{
			ScrapVector[] verts = new ScrapVector[4];
			verts[0] = ScrapMath.RotatePoint(new ScrapVector(Left, Top), Transform.Position, Transform.Rotation);
			verts[1] = ScrapMath.RotatePoint(new ScrapVector(Right, Top), Transform.Position, Transform.Rotation);
			verts[2] = ScrapMath.RotatePoint(new ScrapVector(Right, Bottom), Transform.Position, Transform.Rotation);
			verts[3] = ScrapMath.RotatePoint(new ScrapVector(Left, Bottom), Transform.Position, Transform.Rotation);

			return verts;
		}

		public virtual void Deactivate()
        {
			if (RigidBody.IsStatic)
            {
				Physics2D.StaticBodies.Remove(this);
            }
			else
            {
				Physics2D.DynamicBodies.Remove(this);
            }
        }

		public virtual void Awake()
		{
			TypeOfCollider = ICollider.ColliderType.PIXEL_PERFECT_SAT;
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}

			RigidBody = Owner.GetComponent<RigidBody2D>();
			if (RigidBody == null)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Missing dependency. Requires rigidbody2D component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!RigidBody.IsAwake)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Rigidbody2D component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}

			Sprite = Owner.GetComponent<Sprite2D>();
			if (!RigidBody.IsStatic && Sprite == null)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Missing dependency. Requires Sprite2D component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!RigidBody.IsStatic && !Sprite.IsAwake)
			{
				LogManager.Log(new LogMessage("PixelCollider", "Sprite2D component is not awake... Aborting...", LogMessage.Severity.ERROR));
				return;
			}
			

			if (HasRigidbody)
			{
				if (RigidBody.IsStatic)
				{
					Physics2D.StaticBodies.Add(this);
				}
				else
				{
					Physics2D.DynamicBodies.Add(this);
				}
			}

			IsAwake = true;
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
