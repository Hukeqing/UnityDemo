using System;
using System.Globalization;
using UnityEngine;
using UnityEditor;

namespace DIYMesh.Editor
{
    [CustomEditor(typeof(DiyMeshObject))]
    public class MeshScriptEditor : UnityEditor.Editor
    {
        private SerializedProperty _drawMode;
        private SerializedProperty _mode;

        private DiyMeshObject _this;

        private Color _color = Color.white;
        // float a = 0, b = 0;

        public void OnEnable()
        {
            _drawMode = serializedObject.FindProperty("threeDimensional");
            _mode = serializedObject.FindProperty("mode");
        }

        public override void OnInspectorGUI()
        {
//            _isDemo = EditorGUILayout.Popup("Mode", _isDemo, new[] {"Demo", "Object"});
            var screenSize = new Vector2(Screen.width, Screen.height);
            EditorGUILayout.PropertyField(_drawMode);
            EditorGUILayout.PropertyField(_mode);
            serializedObject.ApplyModifiedProperties();
            if (_mode.enumValueIndex == 1) return;
            (_this = (DiyMeshObject) target).drawCoolDown = EditorGUILayout.Slider("Draw Cool Down", _this.drawCoolDown, 0.05f, 1f);
            if (_this.threeDimensional)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_this.drawBehindWeight.ToString(CultureInfo.CurrentCulture),
                    GUILayout.Width(screenSize.x / 7));
                EditorGUILayout.MinMaxSlider(ref _this.drawBehindWeight,
                    ref _this.drawForwardWeight, -1, 1, GUILayout.Width(screenSize.x * 4 / 7));
                EditorGUILayout.LabelField(_this.drawForwardWeight.ToString(CultureInfo.CurrentCulture),
                    GUILayout.Width(screenSize.x / 7));
                EditorGUILayout.EndHorizontal();
                ;
            }
            else
            {
                _this.drawForwardWeight =
                    EditorGUILayout.Slider(_this.drawForwardWeight.ToString(CultureInfo.CurrentCulture), _this.drawForwardWeight, -1, 1);
            }

            // EditorGUILayout.MinMaxSlider(ref a,  ref b, -10, 10);
            serializedObject.ApplyModifiedProperties();
            _color = EditorGUILayout.ColorField("Color", _color);

            if (!Application.isPlaying) return;
            _this.SetColor(_color, _color.a < 1 ? RenderingMode.Fade : RenderingMode.Opaque);
            if (GUILayout.Button("Clear"))
            {
                _this.Clear();
            }
        }
    }
}