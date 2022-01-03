using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Shapes
{
    public abstract class ScrapShape
    {
        public abstract ScrapVector Center { get; set; }
        public abstract ScrapVector Dimensions { get; set; }

        protected ScrapVector center;
        protected ScrapVector dimensions;

        public ScrapVector[] Verts;

        public abstract ScrapVector[] GetVerticies();

        public static ScrapVector[] GenerateGon(double radius, int sideCount)
        {
            int pointCount = sideCount;

            double theta = ScrapMath.PI2 / pointCount;

            double sin = ScrapMath.Sin(theta);
            double cos = ScrapMath.Cos(theta);

            ScrapVector[] verts = new ScrapVector[pointCount];

            ScrapVector current = new ScrapVector(radius, 0);
            for (int i = 0; i < pointCount; i++)
            {
                ScrapVector nextPoint = new ScrapVector(
                    current.X * cos - current.Y * sin,
                    current.X * sin + current.Y * cos);

                verts[i] = current;

                current = nextPoint;
            }

            return verts;
        }
    }
}
