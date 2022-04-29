using ScrapBox.Framework.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Shapes
{
    public class Rectangle : Polygon
    {
        public Rectangle(ScrapVector position, ScrapVector dimensions)
        {
            verticies = new ScrapVector[4];
            verticies[0] = new ScrapVector(position.X - dimensions.X / 2, position.Y - dimensions.Y / 2);
            verticies[1] = new ScrapVector(position.X + dimensions.X / 2, position.Y - dimensions.Y / 2);
            verticies[2] = new ScrapVector(position.X + dimensions.X / 2, position.Y + dimensions.Y / 2);
            verticies[3] = new ScrapVector(position.X - dimensions.X / 2, position.Y + dimensions.Y / 2);
            

            this.position = position;
            this.dimensions = dimensions;
        }
    }
}
