using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Generic;

namespace ScrapBox.Framework.Shapes
{
    public class ScrapRect : ScrapShape
    {
        public ScrapVector Start;
        public ScrapVector End;

        public double Width { get { return ScrapMath.Abs(End.X - Start.X); } }
        public double Height { get { return ScrapMath.Abs(End.Y - Start.Y); } }

        public double Top { get { return Center.Y - Height / 2; } }
        public double Bottom { get { return Center.Y + Height / 2; } }
        public double Left { get { return Center.X - Width / 2; } }
        public double Right { get { return Center.X + Width / 2; } }

        public override ScrapVector Center { 
            get { return center; }
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
            get { return new ScrapVector(Width, Height); }
            set
            {
                if (dimensions == value)
                    return;

                Start = Center - value / 2;
                End = Center + value / 2;

                Verts = GetVerticies();
            }
        }

        public ScrapRect(ScrapVector start, ScrapVector end)
        {
            if (start.X > end.X && start.Y > end.Y)
            {
                Standard.Swap(ref start, ref end);
            }

            Start = start;
            End = end;

            center = Start + Dimensions / 2;

            Verts = GetVerticies();
        }

        public ScrapRect(int x, int y, int width, int height)
        {
            Start = new ScrapVector(x, y);
            End = Start + new ScrapVector(width, height);

            center = Start + End / 2;

            Verts = GetVerticies();
        }

        public override ScrapVector[] GetVerticies()
        {
            ScrapVector[] verts = new ScrapVector[4];
            verts[0] = new ScrapVector(Left, Top);
            verts[1] = new ScrapVector(Right, Top);
            verts[2] = new ScrapVector(Right, Bottom);
            verts[3] = new ScrapVector(Left, Bottom);

            return verts;
        }

        public static ScrapRect CreateFromCenter(ScrapVector center, ScrapVector dimensions)
        {
            ScrapVector start = center - dimensions / 2;
            ScrapVector end = center + dimensions / 2;

            return new ScrapRect(start, end);
        }

        public static implicit operator Rectangle(ScrapRect r) => new Rectangle((int)r.Start.X, (int)r.Start.Y, (int)r.Width, (int)r.Height);
    }
}
