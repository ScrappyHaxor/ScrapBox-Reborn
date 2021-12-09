using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Scene;

namespace ScrapBox.Framework.ECS.Components
{
	public class BoxCollider2D : Collider
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
			UsedAlgorithm = Algorithm.SAT;

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
