using System;

using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Scene;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.ECS.Components;

namespace ScrapBox.Framework.ECS
{
	public abstract class Collider : IComponent
	{
		public enum Algorithm
		{
			SAT,
			SAT_CIRCLE,
			SAT_PIXEL_PERFECT,
			SAT_PIXEL_PERFECT_CIRCLE
		}

		public enum TriggerType
		{
			NONE,
			TRIGGER_ONLY,
			HYBRID
		}

		public Entity Owner { get; set; }
		public bool IsAwake { get; set; }


		public Transform Transform { get; set; }
		public Sprite2D Sprite { get; set; }
		public ScrapVector Dimensions { get; set; }
		public Algorithm UsedAlgorithm { get; set; }
		public TriggerType Trigger { get; set; }
		public EventHandler<TriggerArgs> Triggered { get; set; }

		public bool HasRigidbody { get { return Owner.HasComponent<RigidBody2D>(); } }
		public double Top { get { return Transform.Position.Y - Dimensions.Y / 2; } }
		public double Bottom { get { return Transform.Position.Y + Dimensions.Y / 2; } }
		public double Left { get { return Transform.Position.X - Dimensions.X / 2; } }
		public double Right { get { return Transform.Position.X + Dimensions.X / 2; } }

        public abstract ScrapVector[] GetVerticies();

        public virtual void Awake()
        {
			Transform = Owner.GetComponent<Transform>();
			if (Transform == null)
			{
				LogManager.Log(new LogMessage("Collider", "Missing dependency. Requires transform component to work.", LogMessage.Severity.ERROR));
				return;
			}

			if (!Transform.IsAwake)
			{
				LogManager.Log(new LogMessage("Collider", "Transform component is not awake... Aborting...", LogMessage.Severity.ERROR));
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

        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
			if (!IsAwake)
				return;

		}
    }
}
