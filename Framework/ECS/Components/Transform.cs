using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.ECS.Components
{
	public class Transform : Component
	{
		public override string Name => "Transform";

        public ScrapVector Position { get; set; }
		public ScrapVector Dimensions { get; set; }
		public float Depth { get; set; }
		public double Rotation { get; set; }
		
		public override void Awake()
		{
			IsAwake = true;
		}

		public override void Sleep()
        {
			IsAwake = false;
        }
	}

}
