using System.Collections.Generic;
using UnityEngine;
using RenderFlow.Shared.Models;

namespace RenderFlow.Features.Collection
{
    public static class RendererCollector
    {
        public static List<RenderFlowItem> Collect(GameObject root)
        {
            var items = new List<RenderFlowItem>();

            if (root == null)
                return items;

            Traverse(root.transform, 0, items);

            return items;
        }

        private static void Traverse(Transform current, int depth, List<RenderFlowItem> items)
        {
            Renderer renderer = current.GetComponent<Renderer>();

            if (renderer != null)
            {
                items.Add(new RenderFlowItem
                {
                    Renderer = renderer,
                    IsSelected = true,
                    Depth = depth
                });
            }

            foreach (Transform child in current)
            {
                Traverse(child, depth + 1, items);
            }
        }
    }
}