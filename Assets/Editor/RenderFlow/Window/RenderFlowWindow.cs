using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RenderFlow.Features.Collection;
using RenderFlow.Shared.Models;

namespace RenderFlow.Window
{
    public class RenderFlowWindow : EditorWindow
    {
        private GameObject _root;
        private List<RenderFlowItem> _items = new();
        private Vector2 _scroll;
        private RenderFlowStyles _styles;

        [MenuItem("Tools/RenderFlow #t")]
        public static void ShowWindow()
        {
            GetWindow<RenderFlowWindow>("RenderFlow");
        }

        private void OnEnable()
        {
            EditorApplication.hierarchyChanged += Refresh;
        }

        private void OnDisable()
        {
            _styles?.Dispose();
            _styles = null;
            EditorApplication.hierarchyChanged -= Refresh;
        }

        private void OnGUI()
        {
            _styles ??= new RenderFlowStyles();
            DrawHeader();
            DrawContent();
        }

        #region Header

        private void DrawHeader()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("RenderFlow", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root", GUILayout.Width(30));
            GameObject newRoot = (GameObject)EditorGUILayout.ObjectField(_root, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            if (newRoot != _root)
            {
                _root = newRoot;
                Refresh();
            }
        }
        
        private const int ToggleWidth = 20;
        private const int OrderWidth = 70;
        private const int LayerWidth = 110;

        private void DrawHeaderRow()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
    
            GUILayout.Label("", GUILayout.Width(ToggleWidth));
            GUILayout.Label("Component", GUILayout.ExpandWidth(true));
            GUILayout.Label("Order", GUILayout.Width(OrderWidth));
            GUILayout.Label("Layer", GUILayout.Width(LayerWidth));
    
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Content

        private void DrawContent()
        {
            if (_root == null)
            {
                EditorGUILayout.HelpBox("Select a root GameObject", MessageType.Info);
                return;
            }

            if (_items == null || _items.Count == 0)
            {
                EditorGUILayout.HelpBox("No Renderers found", MessageType.Warning);
                return;
            }

            DrawHeaderRow();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _items.Count; i++)
                DrawItem(_items[i], i);

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Item

        private void DrawItem(RenderFlowItem item, int index)
        {
            var style = index % 2 == 0 ? _styles.EvenRow : _styles.OddRow;

            EditorGUILayout.BeginHorizontal(style);

            item.IsSelected = EditorGUILayout.Toggle(item.IsSelected, GUILayout.Width(20));
            DrawHierarchyLabel(item);
            EditorGUILayout.LabelField(item.Renderer.sortingOrder.ToString(), GUILayout.Width(60));
            EditorGUILayout.LabelField(item.Renderer.sortingLayerName, GUILayout.Width(100));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHierarchyLabel(RenderFlowItem item)
        {
            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < item.Depth; i++)
                GUILayout.Label("│", GUILayout.Width(10));

            if (item.Depth > 0)
                GUILayout.Label("├─", GUILayout.Width(20));
            else
                GUILayout.Space(20);

            EditorGUILayout.ObjectField(item.Renderer, typeof(Renderer), true);

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        private void Refresh()
        {
            if (_root == null) return;
            _items = RendererCollector.Collect(_root);
            Repaint();
        }
    }
}