using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Shapes
{
    public class Polygon
    {
        public virtual ScrapVector[] Verticies { get { return verticies; } }

        public virtual ScrapVector Position
        {
            get
            {
                return position;
            }
            set
            {
                for (int i = 0; i < verticies.Length; i++)
                {
                    ScrapVector vert = verticies[i];
                    vert -= position;
                    vert += value;
                    verticies[i] = vert;
                }

                position = value;
            }
        }

        public virtual ScrapVector Dimensions
        {
            get { return dimensions; }
            set
            {
                if (value.X < 0 || value.Y < 0)
                    return;

                ScrapVector difference = ScrapMath.Abs(value - dimensions);
                for (int i = 0; i < verticies.Length; i++)
                {
                    ScrapVector vert = verticies[i];
                    vert -= position;
                    double vertX = vert.X;
                    double vertY = vert.Y;

                    if (vertX > 0)
                    {
                        vertX += difference.X / 2;
                    }
                    else if (vertX < 0)
                    {
                        vertX -= difference.X / 2;
                    }

                    if (vertY > 0)
                    {
                        vertY += difference.Y / 2;
                    }
                    else if (vertY < 0)
                    {
                        vertY -= difference.Y / 2;
                    }
                    vert = new ScrapVector(vertX, vertY);
                    vert += position;
                    verticies[i] = vert;
                }

                dimensions = value;
            }
        }

        protected ScrapVector[] verticies;
        protected ScrapVector position;
        protected ScrapVector dimensions;

        public Polygon()
        {

        }

        public void Rotate(double theta)
        {
            theta = ScrapMath.ToRadians(theta);
            for (int i = 0; i < verticies.Length; i++)
            {
                ScrapVector vertex = verticies[i];
                vertex -= position;
                vertex = ScrapMath.RotatePoint(vertex, theta);
                vertex += position;
                verticies[i] = vertex;
            }
        }
    }
}
