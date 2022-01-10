using System;
using System.Collections.Generic;
using System.Linq;

using ScrapBox.Framework.Generic;
using ScrapBox.Framework.Managers;
using ScrapBox.Framework.Services;
using ScrapBox.Framework.Math;

namespace ScrapBox.Framework.Pathfinding
{
    public class NodeMap
    {
        private readonly Dictionary<(double, double), Node> Nodes;

        private double minX;
        private double minY;

        private double maxX;
        private double maxY;

        public double NodeDistance { get; set; }
        public bool DiagonalMovement { get; set; }

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
            Nodes = new Dictionary<(double, double), Node>();
            DiagonalMovement = true;

            minX = double.MaxValue;
            minY = double.MaxValue;

            maxX = double.MinValue;
            maxY = double.MinValue;
        }

        public void RegisterNode(ScrapVector position, bool solid, int weight = 0)
        {
            if (NodeDistance == 0)
            {
                LogService.Log("NodeMap", "RegisterNode", "Node Distance is 0.", Severity.ERROR);
                return;
            }

            if (position.X % NodeDistance != 0 || position.Y % NodeDistance != 0)
            {
                LogService.Log("NodeMap", "RegisterNode", "Node is not on grid", Severity.ERROR);
                return;
            }

            Node newNode = new Node(position, solid, weight);
            Nodes.Add((position.X, position.Y), newNode);

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

        public void UpdateWeight(ScrapVector position, int newWeight)
        {
            if (Nodes.ContainsKey((position.X, position.Y)))
            {
                Nodes[(position.X, position.Y)].Weight = newWeight;
            }
        }

        internal void Purge()
        {
            Nodes.Clear();
        }

        private ScrapVector[] RetraceSteps(Node startNode, Node endNode)
        {
            List<ScrapVector> path = new List<ScrapVector>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            return path.ToArray();
        }

        internal void Navigate(NavigationRequest request)
        {
            if (Nodes.Count == 0)
            {
                LogService.Log("NodeMap", "Navigate", "Node register is empty.", Severity.ERROR);
                PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
                return;
            }

            if (request.Start.X % NodeDistance != 0 || request.Start.Y % NodeDistance != 0 || !Nodes.ContainsKey((request.Start.X, request.Start.Y)))
            {
                LogService.Log("NodeMap", "Navigate", "Start node is invalid or outside NodeMap", Severity.ERROR);
                PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
                return;
            }

            if (request.Target.X % NodeDistance != 0 || request.Target.Y % NodeDistance != 0 || !Nodes.ContainsKey((request.Target.X, request.Target.Y)))
            {
                LogService.Log("NodeMap", "Navigate", "End node is invalid or outside NodeMap", Severity.ERROR);
                PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
                return;
            }

            Node startNode = Nodes[(request.Start.X, request.Start.Y)];
            Node endNode = Nodes[(request.Target.X, request.Target.Y)];

            if (startNode.Blocked)
            {
                LogService.Log("NodeMap", "Navigate", "Start node is blocked", Severity.ERROR);
                PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
                return;
            }

            if (endNode.Blocked)
            {
                LogService.Log("NodeMap", "Navigate", "End node is blocked", Severity.ERROR);
                PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
                return;
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
                    ScrapVector[] path = RetraceSteps(startNode, endNode);
                    PathingManager.FinishedProcessing(new NavigationResult(path, true, request.Callback));
                    return;
                }

                Node[] neighbors = current.Neighbors(Nodes, NodeDistance, DiagonalMovement);
                foreach (Node n in neighbors)
                {
                    if (n == null || n.Blocked || closed.Contains(n))
                        continue;

                    int newNeighborCost = current.GCost + (int)ScrapMath.Distance(current, n) + n.Weight;
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

            PathingManager.FinishedProcessing(new NavigationResult(null, false, request.Callback));
            return;
        }
    }
}
