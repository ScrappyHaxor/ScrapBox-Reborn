using System;
using System.Collections.Generic;
using System.Text;

using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Shapes
{
    public class ScrapPoly : ScrapShape
    {
        public ScrapPoly(ScrapVector[] verts)
        {
            Verts = verts;
        }

        public ScrapPoly(ScrapVector center, double radius, int sideCount)
        {
            Verts = GenerateGon(radius, sideCount);
            for (int i = 0; i < Verts.Length; i++)
            {
                Verts[i] += center;
            }
        }

        public override ScrapVector Center { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ScrapVector Dimensions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override ScrapVector[] GetVerticies()
        {
            return Verts;
        }
    }
}
