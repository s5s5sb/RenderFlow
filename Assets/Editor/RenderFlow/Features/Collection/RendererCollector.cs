using System.Collections.Generic;
using UnityEngine;
using RenderFlow.Shared.Models;

namespace RenderFlow.Features.Collection
{
    public static class RendererCollector
    {
        public static List<RenderFlowItem> Collect(GameObject root)
        {
            List<RenderFlowItem> items = new List<RenderFlowItem>();
            if (root == null) return items;
            Traverse(root.transform, 0, items);
            return items;
        }

        public static void Refresh(GameObject root, List<RenderFlowItem> existing)
        {
            Dictionary<Renderer, RenderFlowItem> existingMap = new Dictionary<Renderer, RenderFlowItem>(existing.Count);
            foreach (var item in existing)
                existingMap[item.Renderer] = item;

            existing.Clear();
            Traverse(root.transform, 0, existing, existingMap);
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
                Traverse(child, depth + 1, items);
        }

        private static void Traverse(Transform current, int depth,
            List<RenderFlowItem> items, Dictionary<Renderer, RenderFlowItem> existingMap)
        {
            Renderer renderer = current.GetComponent<Renderer>();

            if (renderer != null)
            {
                if (existingMap.TryGetValue(renderer, out var existing))
                {
                    existing.Depth = depth;
                    items.Add(existing);
                }
                else
                {
                    items.Add(new RenderFlowItem
                    {
                        Renderer = renderer,
                        IsSelected = false,
                        Depth = depth
                    });
                }
            }

            foreach (Transform child in current)
                Traverse(child, depth + 1, items, existingMap);
        }
    }
}