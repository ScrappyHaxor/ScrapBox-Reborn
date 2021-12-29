using System.Collections.Generic;
using System.Linq;

using ScrapBox.Framework.ECS;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Pathfinding
{
    public class NodeMap
    {
        private readonly List<Node> Nodes;

        public double NodeDistance { get; set; }

        public NodeMap()
        {
            Nodes = new List<Node>();
        }

        public void RegisterNode(ScrapVector position, bool solid)
        {
            Nodes.Add(new Node(position, solid));
        }

        private List<NodeManifold> RetraceSteps(Node startNode, Node endNode)
        {
            List<NodeManifold> path = new List<NodeManifold>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                //if (currentNode != endNode)
                    path.Add(new NodeManifold(currentNode, NodeDistance));

                currentNode = currentNode.Parent;
            }
            
            path.Reverse();
            //path.Add(endNode);

            return path;
        }

        public bool Navigate(ScrapVector a, ScrapVector b, out List<NodeManifold> path, bool diagonalMovement = true)
        {
            path = new List<NodeManifold>();

            if (Nodes.Count == 0)
            {
                LogService.Log("NodeMap", "Navigate", "Node register is empty.", Severity.ERROR);
                return false;
            }

            if (NodeDistance == 0)
            {
                LogService.Log("NodeMap", "Navigate", "Node Distance is 0.", Severity.ERROR);
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

            List<Node> local = Nodes.Select(node => node.Copy()).ToList();

            Node startNode = Node.FindNode(a, local);
            Node endNode = Node.FindNode(b, local);

            if (startNode == null || endNode == null)
            {
                LogService.Log("NodeMap", "Navigate", "Start or end node is invalid or outside NodeMap", Severity.ERROR);
                return false;
            }

            List<Node> open = new List<Node>();
            List<Node> closed = new List<Node>();

            open.Add(startNode);

            while (open.Count > 0)
            {
                Node current = Node.LowestFCost(open);
                open.Remove(current);
                closed.Add(current);

                if (current == endNode)
                {
                    path = RetraceSteps(startNode, endNode);
                    return true;
                }

                Node[] neighbors = current.Neighbors(local, diagonalMovement);
                foreach (Node n in neighbors)
                {
                    if (n == null || n.Blocked || closed.Contains(n))
                        continue;

                    double newNeighborCost = current.GCost + ScrapMath.Distance(current, n);
                    if (newNeighborCost < n.GCost || !open.Contains(n))
                    {
                        n.GCost = newNeighborCost;
                        n.HCost = ScrapMath.Distance(n, endNode);
                        n.Parent = current;

                        if (!open.Contains(n))
                            open.Add(n);
                    }
                }
            }

            return false;
        }
    }
}
