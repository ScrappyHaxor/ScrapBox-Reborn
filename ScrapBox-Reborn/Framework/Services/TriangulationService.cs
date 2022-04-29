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
        public bool Triangulate(ScrapVector[] verticies, TriangulationMethod method, out int[] indicies)
        {
            switch (method)
            {
                case TriangulationMethod.EAR_CLIPPING:
                    bool success = EarClipping(verticies, out indicies);
                    return success;
            }

            LogService.Log("TriangulationService", "Triangulate", "Unknown triangulation method.", Severity.ERROR);
            indicies = null;
            return false;
        }

        public bool EarClipping(ScrapVector[] verticies, out int[] indicies)
        {
            indicies = null;

            if (verticies == null)
            {
                LogService.Log("TriangulationService", "EarClipping", "Array of verticies is null.", Severity.ERROR);
                return false;
            }

            if (verticies.Length < 3)
            {
                LogService.Log("TriangulationService", "EarClipping", "Less than 3 verticies in array.", Severity.ERROR);
                return false;
            }

            List<int> indexList = new List<int>();
            for (int i = 0; i < verticies.Length; i++)
            {
                indexList.Add(i);
            }

            int totalIndexCount = (verticies.Length - 2) * 3;
            indicies = new int[totalIndexCount];

            int currentIndexCount = 0;
            while (indexList.Count > 3)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    int index = Standard.FetchIndex(i, indexList.ToArray());
                    int minusIndex = Standard.FetchIndex(i - 1, indexList.ToArray());
                    int plusIndex = Standard.FetchIndex(i + 1, indexList.ToArray());

                    ScrapVector indexVert = verticies[index];
                    ScrapVector minusIndexVert = verticies[minusIndex];
                    ScrapVector plusIndexVert = verticies[plusIndex];

                    ScrapVector indexVertToMinusVert = minusIndexVert - indexVert;
                    ScrapVector indexVertToPlusVert = plusIndexVert - indexVert;

                    if (ScrapMath.Cross(indexVertToMinusVert, indexVertToPlusVert) < 0)
                        continue;

                    bool isEar = true;
                    ScrapVector[] testEar = new ScrapVector[3];
                    testEar[0] = indexVert;
                    testEar[1] = minusIndexVert;
                    testEar[2] = plusIndexVert;

                    for (int j = 0; j < verticies.Length; j++)
                    {
                        if (j == index || j == minusIndex || j == plusIndex)
                            continue;

                        if (Collision.IntersectPointPolygon(verticies[j], testEar))
                        {
                            isEar = false;
                            break;
                        }
                    }

                    if (isEar)
                    {
                        indicies[currentIndexCount++] = minusIndex;
                        indicies[currentIndexCount++] = index;
                        indicies[currentIndexCount++] = plusIndex;

                        indexList.RemoveAt(i);
                        break;
                    }
                }
            }

            indicies[currentIndexCount++] = indexList[0];
            indicies[currentIndexCount++] = indexList[1];
            indicies[currentIndexCount++] = indexList[2];

            return default;
        }

    }
}
