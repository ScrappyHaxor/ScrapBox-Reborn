using ScrapBox.Framework.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Shapes
{
    public class Triangle : Polygon
    {
        public Triangle(ScrapVector position, ScrapVector dimensions)
        {
            verticies = new ScrapVector[3];
            verticies[0] = new ScrapVector(position.X, position.Y - dimensions.Y / 2);
            verticies[1] = new ScrapVector(position.X - dimensions.X / 2, position.Y + dimensions.Y / 2);
            verticies[2] = new ScrapVector(position.X + dimensions.X / 2, position.Y + dimensions.Y / 2);

            this.position = position;
            this.dimensions = dimensions;
        }
    }
}
