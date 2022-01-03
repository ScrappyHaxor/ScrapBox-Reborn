using System.Collections.Generic;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Generic;

using ScrapBox.Framework.Services;
using System.Diagnostics.CodeAnalysis;

namespace ScrapBox.Framework.Pathfinding
{
    public class Node : ICopyable<Node>, IHeapItem<Node>
    {
        private const double DIAG = 0.7071067811865475;

        public ScrapVector Position { get; set; }
        public bool Blocked { get; set; }

        public Node Parent { get; set; }
        public bool Evalulated { get; set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }

        private int heapIndex;

        public int HeapIndex 
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public Node(ScrapVector position, bool blocked)
        {
            Position = position;
            Blocked = blocked;
        }

        public int CompareTo(Node compNode)
        {
            int comp = FCost.CompareTo(compNode.FCost);
            if (comp == 0)
            {
                comp = HCost.CompareTo(compNode.HCost);
            }

            return -comp;
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

        public static implicit operator ScrapVector(Node n) => n.Position;
    }
}
