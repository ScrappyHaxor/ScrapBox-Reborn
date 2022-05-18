using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.ECS;
using ScrapBox.Framework.ECS.Components;
using System;

namespace ScrapBox.Framework.Level
{
	public class Camera
	{
		public const double MinPlaneDistance = 1;
		public const double MaxPlaneDistance = 1024;

		public const float VirtualWidth = 1820;
		public const float VirtualHeight = 980;

		public ScrapVector Position { get; set; }
		public ScrapVector Origin { get; set; }
		public Rectangle Bounds { get; internal set; }
		public Rectangle VisibleArea { get; internal set; }
		public Matrix ViewMatrix { get; set; }
		public Matrix ProjectionMatrix { get; set; }
		public Matrix TransformationMatrix { get; set; }
		public double Zoom { get; set; }
		public double Rotation { get; set; }
		public double AspectRatio { get; set; }
		public double FieldOfView { get; set; }

		private double optimalZ;
		private double currentZ;

		public Camera(Viewport viewport)
		{
			Position = ScrapVector.Zero;
			Bounds = viewport.Bounds;
			Origin = new ScrapVector(Bounds.Width / 2, Bounds.Height / 2);
			AspectRatio = (double)Bounds.Width / Bounds.Height;
			Zoom = 1;
			FieldOfView = ScrapMath.ToRadians(45);

			optimalZ = (0.5 * Bounds.Height) / ScrapMath.Tan(0.5 * FieldOfView);
			currentZ = optimalZ;
		}

		public bool InView(ScrapVector position, ScrapVector dimensions)
        {
			if (position.X + dimensions.X < VisibleArea.X || position.X - dimensions.X > VisibleArea.X + VisibleArea.Width ||
				position.Y + dimensions.Y < VisibleArea.Y || position.Y - dimensions.Y > VisibleArea.Y + VisibleArea.Height)
			{
				return false;
			}

			return true;
		}
		
		public void Update(Viewport viewport)
		{
			Bounds = viewport.Bounds;
			Origin = new ScrapVector(Bounds.Width / 2, Bounds.Height / 2);
			AspectRatio = (double)Bounds.Width / Bounds.Height;
			optimalZ = (0.5 * Bounds.Height) / ScrapMath.Tan(0.5 * FieldOfView);

			//Update view matrix
			ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, (float)currentZ), new Vector3(0, 0, 0), Vector3.Up);

			//ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)FieldOfView, (float)AspectRatio, (float)MinPlaneDistance, (float)MaxPlaneDistance);
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)FieldOfView, (float)AspectRatio, 0.01f, 100000f);

			//Update transformation matrix
			TransformationMatrix = Matrix.CreateTranslation(new Vector3(-(float)Position.X, -(float)Position.Y, 0)) *
					Matrix.CreateScale(Bounds.Width / VirtualWidth, Bounds.Height / VirtualHeight, 1.0f) * Matrix.CreateScale((float)Zoom, (float)Zoom, 1.0f) * 
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
