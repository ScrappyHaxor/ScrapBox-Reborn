using System.Collections.Generic;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Generic;

using ScrapBox.Framework.Services;
using System.Diagnostics.CodeAnalysis;

namespace ScrapBox.Framework.Pathfinding
{
    public class Node : IHeapItem<Node>
    {
        public ScrapVector Position { get; set; }
        public bool Blocked { get; set; }

        public Node Parent { get; set; }
        public bool Evalulated { get; set; }
        public int Weight { get; set; }
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

        public Node(ScrapVector position, bool blocked, int weight = 0)
        {
            Position = position;
            Blocked = blocked;
            Weight = weight;
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

        public Node[] Neighbors(Dictionary<(double, double), Node> nodes, double NodeDistance, bool diagonal = true)
        {
            Node[] neighbors = new Node[8];

            if (nodes.ContainsKey((Position.X, Position.Y + NodeDistance)))
            {
                neighbors[0] = nodes[(Position.X, Position.Y + NodeDistance)];
            }

            if (nodes.ContainsKey((Position.X, Position.Y - NodeDistance)))
            {
                neighbors[1] = nodes[(Position.X, Position.Y - NodeDistance)];
            }

            if (nodes.ContainsKey((Position.X + NodeDistance, Position.Y)))
            {
                neighbors[2] = nodes[(Position.X + NodeDistance, Position.Y)];
            }

            if (nodes.ContainsKey((Position.X - NodeDistance, Position.Y)))
            {
                neighbors[3] = nodes[(Position.X - NodeDistance, Position.Y)];
            }

            
            if (diagonal)
            {
                if (neighbors[1] != null && neighbors[3] != null && !neighbors[1].Blocked && !neighbors[3].Blocked
                    && neighbors[1].Weight == 0 && neighbors[3].Weight == 0 &&
                    nodes.ContainsKey((Position.X - NodeDistance, Position.Y - NodeDistance)))
                {
                    //Top left
                    neighbors[4] = nodes[(Position.X - NodeDistance, Position.Y - NodeDistance)];
                }

                if (neighbors[1] != null && neighbors[2] != null && !neighbors[1].Blocked && !neighbors[2].Blocked
                    && neighbors[1].Weight == 0 && neighbors[2].Weight == 0 &&
                    nodes.ContainsKey((Position.X + NodeDistance, Position.Y - NodeDistance)))
                {
                    //Top right
                    neighbors[5] = nodes[(Position.X + NodeDistance, Position.Y - NodeDistance)];
                }

                if (neighbors[0] != null && neighbors[3] != null && !neighbors[0].Blocked && !neighbors[3].Blocked
                    && neighbors[0].Weight == 0 && neighbors[3].Weight == 0 &&
                    nodes.ContainsKey((Position.X - NodeDistance, Position.Y + NodeDistance)))
                {
                    //Bottom left
                    neighbors[6] = nodes[(Position.X - NodeDistance, Position.Y + NodeDistance)];
                }

                if (neighbors[0] != null && neighbors[2] != null && !neighbors[0].Blocked && !neighbors[2].Blocked
                    && neighbors[0].Weight == 0 && neighbors[2].Weight == 0 &&
                    nodes.ContainsKey((Position.X + NodeDistance, Position.Y + NodeDistance)))
                {
                    //Bottom right
                    neighbors[7] = nodes[(Position.X + NodeDistance, Position.Y + NodeDistance)];
                }
            }

            return neighbors;
        }

        public static implicit operator ScrapVector(Node n) => n.Position;
    }
}
