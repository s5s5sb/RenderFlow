using System.Collections.Generic;
using System.Linq;
using RenderFlow.Shared.Models;
using UnityEditor;

namespace RenderFlow.Features
{
    public class RendererModifier
    {
        public void SetOrder(List<RenderFlowItem> items, int order)
        {
            foreach (RenderFlowItem item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Set Sorting Order");
                item.Renderer.sortingOrder = order;
                EditorUtility.SetDirty(item.Renderer);
            }
        }

        public void AddOrder(List<RenderFlowItem> items, int delta)
        {
            foreach (RenderFlowItem item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Change Sorting Order");
                item.Renderer.sortingOrder += delta;
                EditorUtility.SetDirty(item.Renderer);
            }
        }

        public void SetLayer(List<RenderFlowItem> items, string layer)
        {
            foreach (RenderFlowItem item in items)
            {
                if (!item.IsSelected) continue;

                Undo.RecordObject(item.Renderer, "Set Sorting Layer");
                item.Renderer.sortingLayerName = layer;
                EditorUtility.SetDirty(item.Renderer);
            }
        }
        
        public void SetSelection(List<RenderFlowItem> items, bool value)
        {
            foreach (RenderFlowItem item in items)
                item.IsSelected = value;
        }

        public List<RenderFlowItem> SortByOrder(List<RenderFlowItem> items, bool ascending)
        {
            return ascending
                ? items.OrderBy(i => i.Renderer.sortingOrder).ToList()
                : items.OrderByDescending(i => i.Renderer.sortingOrder).ToList();
        }
    }
}
