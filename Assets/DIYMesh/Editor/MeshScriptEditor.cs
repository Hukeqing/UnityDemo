using System;
using UnityEngine;
using UnityEditor;

namespace DIYMesh.Editor
{
    [CustomEditor(typeof(DiyMeshObject))]
    public class MeshScriptEditor : UnityEditor.Editor
    {
        private SerializedProperty _drawCoolDown;
        private SerializedProperty _drawWeight;
        private SerializedProperty _mode;
        
        private Color _color = Color.white;
        
        public void OnEnable()
        {
            _drawCoolDown = serializedObject.FindProperty("drawCoolDown");
            _drawWeight = serializedObject.FindProperty("drawWeight");
            _mode = serializedObject.FindProperty("mode");
        }

        public override void OnInspectorGUI()
        {
//            _isDemo = EditorGUILayout.Popup("Mode", _isDemo, new[] {"Demo", "Object"});
            EditorGUILayout.PropertyField(_mode);
            serializedObject.ApplyModifiedProperties();
            if (_mode.enumValueIndex == 1) return;
            EditorGUILayout.PropertyField(_drawCoolDown);
            EditorGUILayout.PropertyField(_drawWeight);
            serializedObject.ApplyModifiedProperties();
            _color = EditorGUILayout.ColorField("Color", _color);
            
            if (!Application.isPlaying) return;
            ((DiyMeshObject)target).SetColor(_color, _color.a < 1 ? RenderingMode.Fade : RenderingMode.Opaque);
            if (GUILayout.Button("Clear"))
            {
                ((DiyMeshObject) target).Clear();
            }
        }
    }
}
