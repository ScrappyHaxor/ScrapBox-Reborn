using ScrapBox.Framework.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Shapes
{
    public class Ellipse : Polygon
    {
        public Ellipse(ScrapVector position, ScrapVector dimensions, int points)
        {
            verticies = new ScrapVector[points-1];

            double step = 2 * ScrapMath.PI / (points - 1);
            int i = 0;
            for (double t = -ScrapMath.PI; t <= ScrapMath.PI; t += step)
            {
                verticies[i] = new ScrapVector(position.X + dimensions.X * ScrapMath.Cos(t), position.Y + dimensions.Y * ScrapMath.Sin(t)); ;
                i++;
            }
        }
    }
}
