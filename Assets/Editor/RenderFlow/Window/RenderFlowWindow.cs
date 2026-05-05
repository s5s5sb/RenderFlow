using System.Collections.Generic;
using System.Linq;
using RenderFlow.Features;
using UnityEditor;
using UnityEngine;
using RenderFlow.Features.Collection;
using RenderFlow.Shared.Models;

namespace RenderFlow.Window
{
    public class RenderFlowWindow : EditorWindow
    {
        private const int ToggleWidth = 20;
        private const int OrderWidth = 70;
        private const int LayerWidth = 110;
        
        private GameObject _root;
        private List<RenderFlowItem> _items = new();
        private Vector2 _scroll;
        private RenderFlowStyles _styles;
        private RendererModifier _rendererModifier;
        private int _orderDelta = 1;
        private int _setOrderValue;
        private int _selectedLayerIndex;
        private string[] _sortingLayers;
        
        [MenuItem("Tools/RenderFlow #t")]
        public static void ShowWindow()
        {
            GetWindow<RenderFlowWindow>("RenderFlow");
        }

        private void OnEnable()
        {
            _rendererModifier = new RendererModifier();
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
            
            if (!CanDrawContent()) return;
            
            DrawModifiers();
            DrawContent();
        }
        
        private bool CanDrawContent()
        {
            if (_root == null)
            {
                EditorGUILayout.HelpBox("Select a root GameObject", MessageType.Info);
                return false;
            }

            if (_items == null || _items.Count == 0)
            {
                EditorGUILayout.HelpBox("No Renderers found", MessageType.Warning);
                return false;
            }

            return true;
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
        
        private void DrawModifiers()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Modify Selection", EditorStyles.boldLabel);

            DrawOrderDelta();
            DrawSetOrder();
            DrawLayer();
        }

        private void DrawLayer()
        {
            if (_sortingLayers == null)
                _sortingLayers = SortingLayer.layers.Select(l => l.name).ToArray();

            EditorGUILayout.BeginHorizontal();

            _selectedLayerIndex = EditorGUILayout.Popup("Layer", _selectedLayerIndex, _sortingLayers);

            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                _rendererModifier.SetLayer(_items, _sortingLayers[_selectedLayerIndex]);
                Refresh();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSetOrder()
        {
            EditorGUILayout.BeginHorizontal();

            _setOrderValue = EditorGUILayout.IntField("Set Order", _setOrderValue);

            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                _rendererModifier.SetOrder(_items, _setOrderValue);
                Refresh();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawOrderDelta()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-10"))
                ApplyDelta(-10);

            if (GUILayout.Button("-1"))
                ApplyDelta(-1);

            _orderDelta = EditorGUILayout.IntField(_orderDelta, GUILayout.Width(50));

            if (GUILayout.Button("+1"))
                ApplyDelta(1);

            if (GUILayout.Button("+10"))
                ApplyDelta(10);
            
            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                ApplyDelta(_orderDelta);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ApplyDelta(int delta)
        {
            _rendererModifier.AddOrder(_items, delta);
            Refresh();
        }
        
        private void Refresh()
        {
            if (_root == null) return;
            _items = RendererCollector.Collect(_root);
            Repaint();
        }

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
            EditorGUILayout.Space(20);
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
    }
}