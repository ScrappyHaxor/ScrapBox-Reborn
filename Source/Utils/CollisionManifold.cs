using System.Collections.Generic;

using ScrapBox.SMath;

namespace ScrapBox.Utils
{
	public class CollisionManifold
	{
		public ScrapVector Normal { get; internal set; }
		public double Depth { get; internal set; }
		public List<ScrapVector> ContactPoints { get; internal set; }

		public CollisionManifold()
		{
			Depth = float.MaxValue;
			ContactPoints = new List<ScrapVector>();
		}

		public CollisionManifold(ScrapVector normal, float depth, List<ScrapVector> contactPoints)
		{
			this.Normal = normal;
			this.Depth = depth;
			this.ContactPoints = contactPoints;
		}
	}
}
