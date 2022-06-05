using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
	public class Transform2D : Component
	{
		public override string Name => "Transform";

        public ScrapVector Position { get; set; }
		public ScrapVector Dimensions { get; set; }
		public float Depth { get; set; }
		public double Rotation { get; set; }
		
		public override void Awake()
		{
			if (IsAwake)
				return;

			if (Layer == null)
				Layer = Owner.Layer;

			IsAwake = true;
		}

		public override void Sleep()
        {
			if (!IsAwake)
				return;

			IsAwake = false;
        }
	}

}
