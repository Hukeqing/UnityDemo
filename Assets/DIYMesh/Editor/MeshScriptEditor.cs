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

        public void OnEnable()
        {
            _drawMode = serializedObject.FindProperty("threeDimensional");
            _mode = serializedObject.FindProperty("mode");
        }

        public override void OnInspectorGUI()
        {
            var screenSize = new Vector2(Screen.width, Screen.height);
            EditorGUILayout.PropertyField(_drawMode);
            EditorGUILayout.PropertyField(_mode);
            serializedObject.ApplyModifiedProperties();
            if (_mode.enumValueIndex == 1) return;
            (_this = (DiyMeshObject) target).drawCoolDown =
                EditorGUILayout.Slider("Draw Cool Down", _this.drawCoolDown, 0.05f, 1f);
            // Rect rect = EditorGUILayout.GetControlRect(true);
            // Debug.Log(rect);
            EditorGUILayout.LabelField("Draw Weight");
            if (_this.threeDimensional)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_this.drawBehindWeight.ToString(CultureInfo.CurrentCulture),
                    GUILayout.Width(screenSize.x * 2 / 15));
                EditorGUILayout.MinMaxSlider(ref _this.drawBehindWeight,
                    ref _this.drawForwardWeight, -1, 1, GUILayout.Width(screenSize.x * 10 / 15));
                EditorGUILayout.LabelField(_this.drawForwardWeight.ToString(CultureInfo.CurrentCulture),
                    GUILayout.Width(screenSize.x * 2 / 15));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _this.drawForwardWeight =
                    EditorGUILayout.Slider(_this.drawForwardWeight.ToString(CultureInfo.CurrentCulture),
                        _this.drawForwardWeight, -1, 1);
            }

            serializedObject.ApplyModifiedProperties();
            _color = EditorGUILayout.ColorField("Color", _color);
            if (Application.isPlaying)
            {
                _this.SetColor(_color, _color.a < 1 ? RenderingMode.Fade : RenderingMode.Opaque);
            }

            EditorGUILayout.BeginHorizontal();
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Clear"))
                {
                    _this.Clear();
                }
            }

            if (_this.threeDimensional)
            {
                if (GUILayout.Button("Reset Left Value"))
                {
                    _this.drawBehindWeight = 0;
                }
            }

            if (GUILayout.Button(_this.threeDimensional ? "Reset Right Value" : "Reset Value"))
            {
                _this.drawForwardWeight = 0;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}