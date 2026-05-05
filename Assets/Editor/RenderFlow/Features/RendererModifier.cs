using System.Collections.Generic;
using RenderFlow.Shared.Models;
using UnityEditor;

namespace RenderFlow.Features
{
    public class RendererModifier
    {
        public void SetOrder(List<RenderFlowItem> items, int order)
        {
            foreach (var item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Set Sorting Order");
                item.Renderer.sortingOrder = order;
                EditorUtility.SetDirty(item.Renderer);
            }
        }

        public void AddOrder(List<RenderFlowItem> items, int delta)
        {
            foreach (var item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Change Sorting Order");
                item.Renderer.sortingOrder += delta;
                EditorUtility.SetDirty(item.Renderer);
            }
        }

        public void SetLayer(List<RenderFlowItem> items, string layer)
        {
            foreach (var item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Set Sorting Layer");
                item.Renderer.sortingLayerName = layer;
                EditorUtility.SetDirty(item.Renderer);
            }
        }
    }
}
