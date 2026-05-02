using UnityEngine;

namespace RenderFlow.Shared.Models
{
    public class RenderFlowItem
    {
        public Renderer Renderer;
        public bool IsSelected;
        public int Depth;

        public string Name => Renderer != null ? Renderer.name : "NULL";
    }
}