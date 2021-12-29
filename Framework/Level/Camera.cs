using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.ECS;
using ScrapBox.Framework.ECS.Components;

namespace ScrapBox.Framework.Level
{
	public class Camera : Entity
	{
        public override string Name => "ScrapBox Camera";
        public Transform Transform { get; set; }
		public Rectangle Bounds { get; internal set; }
		public Rectangle VisibleArea { get; internal set; }
		public Matrix TransformationMatrix { get; set; }
		public double Zoom { get; set; }

		public Camera(Viewport viewport)
		{
			Bounds = viewport.Bounds;
			Zoom = 1;
			Transform = new Transform
            {
                Position = ScrapVector.Zero
            };

            RegisterComponent(Transform);
		}
		
		public void Update(Viewport viewport)
		{
			Bounds = viewport.Bounds;

			//Update transformation matrix
			TransformationMatrix = Matrix.CreateTranslation(
					new Vector3(-(float)Transform.Position.X, -(float)Transform.Position.Y, 0)) *
					Matrix.CreateScale((float)Zoom) * 
					Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));

			//Update visible area
			Matrix inverse = Matrix.Invert(TransformationMatrix);

			ScrapVector topLeft = ScrapVector.Transform(ScrapVector.Zero, inverse);
			ScrapVector topRight = ScrapVector.Transform(new ScrapVector(Bounds.X, 0), inverse);
			ScrapVector bottomLeft = ScrapVector.Transform(new ScrapVector(0, Bounds.Y), inverse);
			ScrapVector bottomRight = ScrapVector.Transform(new ScrapVector(Bounds.Width, Bounds.Height), inverse);

			ScrapVector min = new ScrapVector(
					ScrapMath.Min(topLeft.X, ScrapMath.Min(topRight.X, ScrapMath.Min(bottomLeft.X, bottomRight.X))),
					ScrapMath.Min(topLeft.Y, ScrapMath.Min(topRight.Y, ScrapMath.Min(bottomLeft.Y, bottomRight.Y))));

			ScrapVector max = new ScrapVector(
					ScrapMath.Max(topLeft.X, ScrapMath.Max(topRight.X, ScrapMath.Max(bottomLeft.X, bottomRight.X))),
					ScrapMath.Max(topLeft.Y, ScrapMath.Max(topRight.Y, ScrapMath.Max(bottomLeft.Y, bottomRight.Y))));

			VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

		}
	}
}
