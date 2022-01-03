using System.Collections.Generic;
using System.Linq;

using ScrapBox.Framework.Generic;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Pathfinding
{
    public class NodeMap
    {
        private readonly List<Node> Nodes;

        private double minX;
        private double minY;

        private double maxX;
        private double maxY;

        public double NodeDistance { get; set; }

        public double MaxSize
        {
            get
            {
                return ScrapMath.Distance(new ScrapVector(minX, maxY), new ScrapVector(maxX, maxY)) / NodeDistance *
                    ScrapMath.Distance(new ScrapVector(minX, maxY), new ScrapVector(minX, minY)) / NodeDistance;
            }
        }

        public NodeMap()
        {
            Nodes = new List<Node>();

            minX = double.MaxValue;
            minY = double.MaxValue;

            maxX = double.MinValue;
            maxY = double.MinValue;
        }

        public void RegisterNode(ScrapVector position, bool solid)
        {
            if (NodeDistance == 0)
            {
                LogService.Log("NodeMap", "Navigate", "Node Distance is 0.", Severity.ERROR);
                return;
            }

            Node newNode = new Node(position, solid);
            Nodes.Add(newNode);


            if (newNode.Position.X < minX)
            {
                minX = newNode.Position.X - NodeDistance * 4;
            }

            if (newNode.Position.Y < minY)
            {
                minY = newNode.Position.Y - NodeDistance * 4;
            }

            if (newNode.Position.X > maxX)
            {
                maxX = newNode.Position.X + NodeDistance * 4;
            }

            if (newNode.Position.Y > maxY)
            {
                maxY = newNode.Position.Y + NodeDistance * 4;
            }
        }

        private List<ScrapVector> RetraceSteps(Node startNode, Node endNode)
        {
            List<ScrapVector> path = new List<ScrapVector>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            
            path.Reverse();

            return path;
        }

        public bool Navigate(ScrapVector a, ScrapVector b, out List<ScrapVector> path, bool diagonalMovement = true)
        {
            path = new List<ScrapVector>();

            if (Nodes.Count == 0)
            {
                LogService.Log("NodeMap", "Navigate", "Node register is empty.", Severity.ERROR);
                return false;
            }

            if (a.X % NodeDistance != 0 || a.Y % NodeDistance != 0)
            {
                LogService.Log("NodeMap", "Navigate", "Start node is not on grid.", Severity.WARNING);
            }

            if (b.X % NodeDistance != 0 || b.Y % NodeDistance != 0)
            {
                LogService.Log("NodeMap", "Navigate", "End node is not on grid.", Severity.WARNING);
            }

            Node startNode = Node.FindNode(a, Nodes);
            Node endNode = Node.FindNode(b, Nodes);

            if (startNode == null || endNode == null)
            {
                LogService.Log("NodeMap", "Navigate", "Start or end node is invalid or outside NodeMap", Severity.ERROR);
                return false;
            }

            Heap<Node> open = new Heap<Node>((int)MaxSize);
            Heap<Node> closed = new Heap<Node>((int)MaxSize);

            open.Add(startNode);

            while (open.Count > 0)
            {
                Node current = open.RemoveFirst();
                closed.Add(current);

                if (current == endNode)
                {
                    path = RetraceSteps(startNode, endNode);
                    return true;
                }

                Node[] neighbors = current.Neighbors(Nodes, diagonalMovement);
                foreach (Node n in neighbors)
                {
                    if (n == null || n.Blocked || closed.Contains(n))
                        continue;

                    int newNeighborCost = current.GCost + (int)ScrapMath.Distance(current, n);
                    if (newNeighborCost < n.GCost || !open.Contains(n))
                    {
                        n.GCost = newNeighborCost;
                        n.HCost = (int)ScrapMath.Distance(n, endNode);
                        n.Parent = current;

                        if (!open.Contains(n))
                        {
                            open.Add(n);
                        }
                        else
                        {
                            open.Update(n);
                        }
                    }
                }
            }

            return false;
        }
    }
}
