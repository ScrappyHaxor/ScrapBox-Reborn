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
        public ScrapVector[] Triangulate(ScrapVector[] verts, TriangulationMethod method)
        {
            switch (method)
            {
                case TriangulationMethod.EAR_CLIPPING:
                    return EarClipping(verts);
            }

            return default;
        }

        public ScrapVector[] EarClipping(ScrapVector[] verts)
        {
            if (verts == null)
            {
                LogService.Log("TriangulationService", "EarClipping", "Array of verticies is null.", Severity.ERROR);
                return verts;
            }

            if (verts.Length < 3)
            {
                LogService.Log("TriangulationService", "EarClipping", "Less than 3 verticies in array.", Severity.ERROR);
                return verts;
            }

            int triangleCount = verts.Length - 2;
            int totalTriangleIndexCount = triangleCount * 3;



            List<ScrapVector> workingSet = new List<ScrapVector>(verts);
            while (workingSet.Count > 3)
            {
                
            }

            return default;
        }

    }
}
