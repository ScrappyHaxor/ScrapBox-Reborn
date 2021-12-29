using System.Collections.Generic;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Generic;

using ScrapBox.Framework.Services;

namespace ScrapBox.Framework.Pathfinding
{
    public class NodeManifold
    {
        public readonly ScrapVector nodeCenter;
        public readonly ScrapVector nodeTop;
        public readonly ScrapVector nodeBottom;
        public readonly ScrapVector nodeLeft;
        public readonly ScrapVector nodeRight;

        public readonly ScrapVector nodeTopRight;
        public readonly ScrapVector nodeTopLeft;
        public readonly ScrapVector nodeBottomRight;
        public readonly ScrapVector nodeBottomLeft;

        public NodeManifold(Node n, double nodeDistance)
        {
            nodeCenter = n.Position;
            nodeTop = new ScrapVector(n.Position.X, n.Position.Y - nodeDistance / 2);
            nodeBottom = new ScrapVector(n.Position.X, n.Position.Y + nodeDistance / 2);
            nodeLeft = new ScrapVector(n.Position.X - nodeDistance / 2, n.Position.Y);
            nodeRight = new ScrapVector(n.Position.X + nodeDistance / 2, n.Position.Y);

            nodeTopRight = nodeTop + nodeRight;
            nodeTopLeft = nodeTop + nodeLeft;
            nodeBottomRight = nodeBottom + nodeRight;
            nodeBottomLeft = nodeBottom + nodeLeft;
        }

        public List<ScrapVector> Points()
        {
            List<ScrapVector> points = new List<ScrapVector>();
            points.Add(nodeCenter);
            points.Add(nodeTop);
            points.Add(nodeBottom);
            points.Add(nodeLeft);
            points.Add(nodeRight);

            points.Add(nodeTopRight);
            points.Add(nodeTopLeft);
            points.Add(nodeBottomRight);
            points.Add(nodeBottomLeft);

            return points;
        }

        public static implicit operator ScrapVector(NodeManifold n) => n.nodeCenter;
    }

    public class Node : ICopyable<Node>
    {
        private const double DIAG = 0.7071067811865475;

        public ScrapVector Position { get; set; }
        public bool Blocked { get; set; }

        public Node Parent { get; set; }
        public bool Evalulated { get; set; }
        public double GCost { get; set; }
        public double HCost { get; set; }
        public double FCost { get { return GCost + HCost; } }


        public Node(ScrapVector position, bool blocked)
        {
            Position = position;
            Blocked = blocked;
        }

        public Node Copy()
        {
            return new Node(Position, Blocked);
        }

        public Node[] Neighbors(List<Node> nodes, bool diagonal = true)
        {
            Node[] neighbors = new Node[8];

            neighbors[0] = ClosestInDirection(new ScrapVector(0, 1), nodes); // down
            neighbors[1] = ClosestInDirection(new ScrapVector(0, -1), nodes); // up
            neighbors[2] = ClosestInDirection(new ScrapVector(1, 0), nodes); // right
            neighbors[3] = ClosestInDirection(new ScrapVector(-1, 0), nodes); // left
            
            if (diagonal)
            {
                if (neighbors[1] != null && neighbors[3] != null && !neighbors[1].Blocked && !neighbors[3].Blocked)
                {
                    //Top left
                    neighbors[4] = ClosestInDirection(new ScrapVector(-DIAG, -DIAG), nodes);
                }

                if (neighbors[1] != null && neighbors[2] != null && !neighbors[1].Blocked && !neighbors[2].Blocked)
                {
                    //Top right
                    neighbors[5] = ClosestInDirection(new ScrapVector(DIAG, -DIAG), nodes);
                }

                if (neighbors[0] != null && neighbors[3] != null && !neighbors[0].Blocked && !neighbors[3].Blocked)
                {
                    //Bottom left
                    neighbors[6] = ClosestInDirection(new ScrapVector(-DIAG, DIAG), nodes); //Bottom right
                }

                if (neighbors[0] != null && neighbors[2] != null && !neighbors[0].Blocked && !neighbors[2].Blocked)
                {
                    //Bottom right
                    neighbors[7] = ClosestInDirection(new ScrapVector(DIAG, DIAG), nodes); // Bottom left
                }
            }

            return neighbors;
        }

        public Node ClosestInDirection(ScrapVector direction, List<Node> nodes)
        {
            double minDistance = double.MaxValue;
            int index = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                Node n = nodes[i];
                if (ScrapMath.Normalize(n - Position) != direction)
                    continue;

                double d = ScrapMath.Distance(n, Position);
                if (d < minDistance)
                {
                    minDistance = d;
                    index = i;
                }
            }

            if (index == -1)
                return null;

            return nodes[index];
        }

        public static Node FindNode(ScrapVector position, List<Node> nodes)
        {
            foreach (Node n in nodes)
            {
                if (n == position)
                    return n;
            }

            return null;
        }

        public static Node LowestFCost(List<Node> nodes)
        {
            double lowest = double.MaxValue;
            Node cheapest = null;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                Node n = nodes[i];

                if (n.FCost < lowest || (n.FCost == lowest && n.HCost < cheapest.HCost))
                {
                    cheapest = nodes[i];
                    lowest = n.FCost;
                }
            }

            return cheapest;
        }

        public static implicit operator ScrapVector(Node n) => n.Position;
    }
}
