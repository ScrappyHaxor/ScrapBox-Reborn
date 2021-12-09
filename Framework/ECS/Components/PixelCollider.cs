using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Math;
using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS.Components
{
	public class PixelCollider : Collider
	{
		public override ScrapVector[] GetVerticies()
		{
			ScrapVector[] verts = new ScrapVector[4];
			verts[0] = ScrapMath.RotatePoint(new ScrapVector(Left, Top), Transform.Position, Transform.Rotation);
			verts[1] = ScrapMath.RotatePoint(new ScrapVector(Right, Top), Transform.Position, Transform.Rotation);
			verts[2] = ScrapMath.RotatePoint(new ScrapVector(Right, Bottom), Transform.Position, Transform.Rotation);
			verts[3] = ScrapMath.RotatePoint(new ScrapVector(Left, Bottom), Transform.Position, Transform.Rotation);

			return verts;
		}

		public override void Awake()
		{
			UsedAlgorithm = Algorithm.SAT_PIXEL_PERFECT;

			Sprite = Owner.GetComponent<Sprite2D>();
			RigidBody = Owner.GetComponent<RigidBody2D>();
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

			base.Awake();
		}

		public override void Update(double dt)
		{
			if (!IsAwake)
				return;

			base.Update(dt);
		}

		public override void Draw(SpriteBatch spriteBatch, Camera camera)
		{
			if (!IsAwake)
				return;

			base.Draw(spriteBatch, camera);
		}
	}
}
