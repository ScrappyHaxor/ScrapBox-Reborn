using ScrapBox.Framework.Generic;
using ScrapBox.Framework.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrapBox.Framework.Services
{
    public enum TriangulationMethod
    {
        EAR_CLIPPING
    }

    public class TriangulationService
    {
        public void Triangulate(ScrapVector[] verts, TriangulationMethod method)
        {
            switch (method)
            {
                case TriangulationMethod.EAR_CLIPPING:
                    EarClipping(verts);
                    break;
            }
        }

        public void EarClipping(ScrapVector[] verts)
        {
            List<ScrapVector> trianglePositions;

            if (verts == null)
            {
                LogService.Log("TriangulationService", "EarClipping", "Array of verticies is null.", Severity.ERROR);
                return;
            }

            if (verts.Length < 3)
            {
                LogService.Log("TriangulationService", "EarClipping", "Less than 3 verticies in array.", Severity.ERROR);
            }

            List<ScrapVector> workingSet = new List<ScrapVector>(verts);
            for (int i = 0; i < verts.Length; i++)
            {
                ScrapVector p1 = Standard.FetchIndex(i - 1, verts);
                ScrapVector p2 = Standard.FetchIndex(i, verts);
                ScrapVector p3 = Standard.FetchIndex(i + 1, verts);


            }
        }

    }
}
