using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.Shapes
{
    public class ScrapCircle : ScrapShape
    {
        public const int CIRCLE_MIN_POINTS = 8;

        public override ScrapVector Center 
        { get { return center; } 
            set
            {
                for (int i = 0; i < Verts.Length; i++)
                {
                    Verts[i] -= center;
                    Verts[i] += value;
                }

                center = value;
            }
        }

        public override ScrapVector Dimensions { 
            get { return new ScrapVector(Radius, Radius); }
            set
            {
                if (value.X == Radius)
                    return;

                Radius = value.X;
                Verts = GenerateGon(Radius, points);
                for (int i = 0; i < Verts.Length; i++)
                {
                    Verts[i] += center;
                }
            }
        }

        public double Radius;
        private readonly int points;

        public ScrapCircle(ScrapVector position, double radius, int points)
        {
            center = position;
            Radius = radius;
            this.points = points;
            points = (int)ScrapMath.Clamp(points, CIRCLE_MIN_POINTS, Renderer.MAX_CIRCLE_POINTS);

            Verts = GenerateGon(radius, points);
            for (int i = 0; i < Verts.Length; i++)
            {
                Verts[i] += position;
            }
        }

        public override ScrapVector[] GetVerticies()
        {
            return Verts;
        }
    }
}
