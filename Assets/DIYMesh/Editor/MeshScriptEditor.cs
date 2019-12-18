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
        
        private bool _isDemo = true;
        private Color _color = Color.red;
        
        public void OnEnable()
        {
            _drawCoolDown = serializedObject.FindProperty("drawCoolDown");
            _drawWeight = serializedObject.FindProperty("drawWeight");
        }

        public override void OnInspectorGUI()
        {
//            base.OnInspectorGUI();
//            serializedObject.Update();
            _isDemo = EditorGUILayout.Foldout(_isDemo, "OnDemo");
            if (!_isDemo) return;
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