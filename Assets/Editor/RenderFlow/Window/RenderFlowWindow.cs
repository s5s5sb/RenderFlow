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
        private const string LogoPath = "Assets/Editor/RenderFlow/Logo.png";
        private const int ToggleWidth = 20;
        private const int OrderWidth = 70;
        private const int LayerWidth = 110;
        
        private Texture2D _logo;
        private GameObject _root;
        private List<RenderFlowItem> _items = new();
        private Vector2 _scroll;
        private RenderFlowStyles _styles;
        private RendererModifier _rendererModifier;
        private int _orderDelta = 1;
        private int _setOrderValue;
        private int _selectedLayerIndex;
        private bool _sortAscending = true;
        private bool _sortActive;
        
        [MenuItem("Tools/RenderFlow #t")]
        public static void ShowWindow()
        {
            GetWindow<RenderFlowWindow>("RenderFlow");
        }

        private void OnEnable()
        {
            SetLogo();
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

        private void SetLogo()
        {
            _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
        }

        private void DrawLogo()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            GUILayout.Label(_logo, GUILayout.Height(50));
            EditorGUILayout.EndHorizontal();
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
            DrawLogo();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Root", GUILayout.Width(30));
            GameObject newRoot = (GameObject)EditorGUILayout.ObjectField(_root, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            if (newRoot != _root)
            {
                _root = newRoot;
                _sortAscending = true;
                _sortActive = false;
                _items = RendererCollector.Collect(_root);
                Repaint();
            }
        }
        
        private void DrawModifiers()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Modify Selection", EditorStyles.boldLabel);

            DrawOrderDelta();
            DrawSetOrder();
            DrawLayer();

            EditorGUILayout.Space(10);
            DrawSelectionControls();
        }

        private void DrawLayer()
        {
            string[] sortingLayers = SortingLayer.layers.Select(l => l.name).ToArray();

            EditorGUILayout.BeginHorizontal();

            _selectedLayerIndex = EditorGUILayout.Popup("Layer", _selectedLayerIndex, sortingLayers);

            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                _rendererModifier.SetLayer(_items, sortingLayers[_selectedLayerIndex]);
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

            EditorGUILayout.LabelField("Change Order", GUILayout.Width(150));
            
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
            RendererCollector.Refresh(_root, _items);
    
            if (_sortActive)
                _items = _rendererModifier.SortByOrder(_items, !_sortAscending);
    
            Repaint();
        }
        
        private void DrawSelectionControls()
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select All"))
                _rendererModifier.SetSelection(_items, true);

            if (GUILayout.Button("Unselect All"))
                _rendererModifier.SetSelection(_items, false);

            string sortLabel = _sortAscending ? "Sort by Order ↓" : "Sort by Order ↑";
            if (GUILayout.Button(sortLabel))
            {
                _sortActive = true;
                _items = _rendererModifier.SortByOrder(_items, _sortAscending);
                _sortAscending = !_sortAscending;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHeaderRow()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
    
            GUILayout.Label("", GUILayout.Width(ToggleWidth));
            GUILayout.Label("Component", GUILayout.ExpandWidth(true));
            
            string sortLabel = "Order";
            string sortPrefix = "";
            if (_sortActive)
            {
                sortPrefix = _sortAscending ? "↑" : "↓";
            }
            GUILayout.Label(sortLabel + sortPrefix, GUILayout.Width(OrderWidth));
            GUILayout.Label("Layer", GUILayout.Width(LayerWidth));
    
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Content

        private void DrawContent()
        {
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