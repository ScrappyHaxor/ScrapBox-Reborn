using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Level;

namespace ScrapBox.Framework.ECS.Components
{
	public class BoxCollider2D : Collider
	{
		public override string Name => "BoxCollider2D";

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
			Algorithm = CollisionAlgorithm.SAT;

			base.Awake();
		}
	}
}
