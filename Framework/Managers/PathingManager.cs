using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

using ScrapBox.Framework.Math;
using ScrapBox.Framework.Pathfinding;

namespace ScrapBox.Framework.Managers
{
    public class NavigationRequest
    {
        public ScrapVector Start;
        public ScrapVector Target;
        public Action<bool, ScrapVector[]> Callback;

        public NavigationRequest(ScrapVector start, ScrapVector target, Action<bool, ScrapVector[]> callback)
        {
            Start = start;
            Target = target;
            Callback = callback;
        }
    }

    public class NavigationResult
    {
        public ScrapVector[] Path;
        public bool Success;
        public Action<bool, ScrapVector[]> Callback;

        public NavigationResult(ScrapVector[] path, bool success, Action<bool, ScrapVector[]> callback)
        {
            Path = path;
            Success = success;
            Callback = callback;
        }
    }

    public static class PathingManager
    {
        private static readonly Queue<NavigationResult> resultQueue;

        internal static NodeMap map;

        static PathingManager()
        {
            resultQueue = new Queue<NavigationResult>();
        }

        internal static void Update()
        {
            if (resultQueue.Count > 0)
            {
                int itemsInQueue = resultQueue.Count;
                lock (resultQueue)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        NavigationResult result = resultQueue.Dequeue();
                        result.Callback(result.Success, result.Path);
                    }
                }
            }
        }

        internal static void FinishedProcessing(NavigationResult result)
        {
            lock(result)
            {
                resultQueue.Enqueue(result);
            }
        }

        public static void RequestNavgiation(NavigationRequest request)
        {
            ThreadStart refThread = delegate
            {
                map.Navigate(request);
            };

            refThread.Invoke();
        }
    }
}
