using ScrapBox.Framework.Math;

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
			if (IsAwake)
				return;

			Algorithm = CollisionAlgorithm.SAT_PIXEL;

			bool success = Dependency(out Sprite);
			if (!success)
				return;

			base.Awake();
		}
	}
}
