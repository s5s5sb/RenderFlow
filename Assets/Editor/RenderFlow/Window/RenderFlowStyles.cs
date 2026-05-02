using UnityEditor;
using UnityEngine;

namespace RenderFlow.Window
{
    public class RenderFlowStyles : System.IDisposable
    {
        public GUIStyle EvenRow { get; }
        public GUIStyle OddRow { get; }

        private readonly Texture2D _evenTex;
        private readonly Texture2D _oddTex;

        public RenderFlowStyles()
        {
            _evenTex = MakeTex(new Color(0, 0, 0, 0.15f));
            _oddTex  = MakeTex(new Color(0, 0, 0, 0f));

            EvenRow = new GUIStyle(EditorStyles.helpBox) { normal = { background = _evenTex } };
            OddRow  = new GUIStyle(EditorStyles.helpBox) { normal = { background = _oddTex  } };
        }

        public void Dispose()
        {
            if (_evenTex != null) Object.DestroyImmediate(_evenTex);
            if (_oddTex  != null) Object.DestroyImmediate(_oddTex);
        }

        private static Texture2D MakeTex(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();
            return tex;
        }
    }
}